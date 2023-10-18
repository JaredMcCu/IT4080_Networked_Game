using System;
using UnityEngine;
using UnityEngine.UIElements;


public class PlayerCard : VisualElement
{
    public new class UxmlFactory : UxmlFactory<PlayerCard> {}

    private Label lblPlayerName;
    private Label lblStatus;
    private VisualElement elemColor;
    private Button btnKick;

    /// <summary>
    /// The player's name.  Call UpdateDisplay for it to be reflected in the UI.
    /// </summary>
    public string playerName = "Not Set";
    /// <summary>
    /// Whether the player is ready.  Call UpdateDisplay for it to be reflected
    /// in the UI.
    /// </summary>
    public bool ready = false;
    /// <summary>
    /// The player's color.  Call UpdateDisplay for it to be reflected in the UI.
    /// </summary>
    public Color color = Color.magenta;
    /// <summary>
    /// The player's clientID.  Call UpdateDisplay for it to be reflected in the UI.
    /// </summary>
    public ulong clientId = ulong.MaxValue;

    /// <summary>
    /// Fired when the Kick button is pressed for this card.  The player's
    /// clientId is sent with the event.
    /// </summary>
    public event Action<ulong> OnKickClicked;

    public PlayerCard() { }

    public void Init()
    {
        VisualElement tr = this.Q<VisualElement>("top-row");
        lblPlayerName = tr.Q<Label>("player-name");
        lblStatus = tr.Q<Label>("status");
        btnKick = this.Q<Button>();
        btnKick.clicked += BtnKick_clicked;

        elemColor = this.Q<VisualElement>("bottom-row").Q<VisualElement>("color");

        UpdateDisplay();
    }


    private void BtnKick_clicked() {
        OnKickClicked?.Invoke(clientId);
    }


    /// <summary>
    /// Call this to see updated values in the UI.
    /// </summary>
    public void UpdateDisplay()
    {
        string strClientId = clientId.ToString();
        if(clientId == ulong.MaxValue)
        {
            strClientId = "?";
        }
        lblPlayerName.text = $"({strClientId}) {playerName}";
        elemColor.style.backgroundColor = color;
        if (ready) {
            lblStatus.text = "Ready!!";
        } else {
            lblStatus.text = "NOT READY";
        }
    }


    /// <summary>
    /// Show/hide the kick button.
    /// </summary>
    /// <param name="should"></param>
    public void ShowKick(bool should)
    {
        btnKick.visible = should;
    }
}