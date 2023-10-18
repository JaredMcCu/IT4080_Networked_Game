using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Lobby : NetworkBehaviour
{
    public LobbyUi lobbyUi;
    public NetworkedPlayers networkedPlayers;

    void Start()
    {
        if (IsServer)
        {
            networkedPlayers.allNetPlayers.OnListChanged += ServerOnNetworkedPlayersChanged;
            ServerPopulateCards();
        }
    }

    private void ServerOnNetworkedPlayersChanged(NetworkListEvent<NetworkPlayerInfo> changeEvent) {
        ServerPopulateCards();
    }

    private void ServerPopulateCards() 
    {
        lobbyUi.playerCards.Clear();
        foreach(NetworkPlayerInfo info in networkedPlayers.allNetPlayers)
        {
            PlayerCard pc = lobbyUi.playerCards.AddCard("Some player");
            pc.ready = info.ready;
            pc.clientId = info.clientId;
            pc.UpdateDisplay();
        }
    }
}
