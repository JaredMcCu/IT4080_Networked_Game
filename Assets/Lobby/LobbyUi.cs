using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyUi : MonoBehaviour
{
    /// <summary>
    /// Fired when Ready Toggle is toggled.  The new value is passed.
    /// </summary>
    public event Action<bool> OnReadyToggled;
    /// <summary>
    /// Fired when the Start button is clicked
    /// </summary>
    public event Action OnStartClicked;
    /// <summary>
    /// Fired when the name field's change button is clicked.  The value
    /// of the field is passed.
    /// </summary>
    public event Action<string> OnChangeNameClicked;

    /// <summary>
    /// Container and methods for handling a list of PlayerCard.
    /// </summary>
    public PlayerCards playerCards;

    private VisualElement root;
    private VisualElement playerControls;
    private Button btnStart;
    private Toggle tglReady;
    private TextField txtPlayerName;
    private Button btnChangeName;


    private void Awake() {
        SetupControls();
    }


    private void SetupControls() {
        root = GetComponent<UIDocument>().rootVisualElement;
        playerControls = root.Q<VisualElement>("right").Q<VisualElement>("player-controls");
        btnStart = playerControls.Q<Button>("Start");
        tglReady = playerControls.Q<Toggle>("Ready");
        txtPlayerName = playerControls.Q<TextField>("player-name");
        btnChangeName = playerControls.Q<Button>("change-name");
        playerCards = root.Q<PlayerCards>();

        btnStart.clicked += BtnStart_clicked;
        btnChangeName.clicked += BtnChangeName_clicked;
        tglReady.RegisterValueChangedCallback(evt =>
        {
            OnReadyToggled?.Invoke(evt.newValue);
        });
    }


    // ----------------------
    // Events
    // ----------------------
    private void BtnChangeName_clicked() {
        OnChangeNameClicked?.Invoke(txtPlayerName.value);
    }


    private void BtnStart_clicked() {
        OnStartClicked?.Invoke();
    }


    // ----------------------
    // Public
    // ----------------------
    /// <summary>
    /// Show/hide the start button and ready toggle.  When the start button is
    /// visible, the toggle is not, and vice versa.
    /// </summary>
    /// <param name="should"></param>
    public void ShowStart(bool should) {
        btnStart.visible = should;
        tglReady.visible = !should;
    }


    /// <summary>
    /// Sets the value of the player's name text field.
    /// </summary>
    /// <param name="pname"></param>
    public void SetPlayerName(string pname) {
        txtPlayerName.value = pname;
    }


    /// <summary>
    /// Enable/disable the start button.
    /// </summary>
    /// <param name="should"></param>
    public void EnableStart(bool should)
    {
        btnStart.SetEnabled(should);
    }
}
