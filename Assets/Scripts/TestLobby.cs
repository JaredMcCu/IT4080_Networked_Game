using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLobby : MonoBehaviour
{
    public LobbyUi lobbyUi;

    // Start is called before the first frame update
    void Start()
    {
        CreateTestCards();
        lobbyUi.OnReadyToggled += TestOnReadyToggled;
        lobbyUi.OnStartClicked += TestOnStartClicked;
        lobbyUi.OnChangeNameClicked += TestOnChangeNameClicked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateTestCards()
    {
        PlayerCard pc = lobbyUi.playerCards.AddCard("test player 1");
        pc.color = Color.grey;
        pc.ready = true;
        pc.ShowKick(false);
        pc.clientId = 99;
        pc.OnKickClicked += TestOnKickClicked;
        pc.UpdateDisplay();

        pc = lobbyUi.playerCards.AddCard("test player 2");
        pc.color = Color.green;
        pc.ready = false;
        pc.ShowKick(true);
        pc.clientId = 50;
        pc.OnKickClicked += TestOnKickClicked;
        pc.UpdateDisplay();
    }

    private void TestOnKickClicked(ulong clientId)
    {
        Debug.Log($"We wanna kick client {clientId}");
    }

    private void TestOnReadyToggled(bool newValue)
    {
        Debug.Log($"Ready = {newValue}");
    }

    private void TestOnStartClicked()
    {
        lobbyUi.ShowStart(false);
    }

    private void TestOnChangeNameClicked(string newName)
    {
        Debug.Log($"new name = {newName}");
    }
}
