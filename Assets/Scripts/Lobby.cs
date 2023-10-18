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
            lobbyUi.ShowStart(true);
        } else {
            ClientPopulateCards();
            networkedPlayers.allNetPlayers.OnListChanged += ClientNetPlayerChanged;
            lobbyUi.ShowStart(false);
            lobbyUi.OnReadyToggled += ClientOnReadyToggled;
        }
    }

    private void ServerOnNetworkedPlayersChanged(NetworkListEvent<NetworkPlayerInfo> changeEvent) {
        ServerPopulateCards();
    }

    private void ClientNetPlayerChanged(NetworkListEvent<NetworkPlayerInfo> changeEvent) {
        ClientPopulateCards();
    }

    private void ServerPopulateCards() 
    {
        lobbyUi.playerCards.Clear();
        foreach(NetworkPlayerInfo info in networkedPlayers.allNetPlayers)
        {
            PlayerCard pc = lobbyUi.playerCards.AddCard("Some player");
            pc.ready = info.ready;
            pc.clientId = info.clientId;
            pc.color = info.color;
            if(info.clientId == NetworkManager.LocalClientId)
            {
                pc.ShowKick(false);
            } else {
                pc.ShowKick(true);
            }
            pc.UpdateDisplay();
        }
    }

    private void ClientPopulateCards() 
    {
        lobbyUi.playerCards.Clear();
        foreach(NetworkPlayerInfo info in networkedPlayers.allNetPlayers)
        {
            PlayerCard pc = lobbyUi.playerCards.AddCard("Some player");
            pc.ready = info.ready;
            pc.clientId = info.clientId;
            pc.color = info.color;
            pc.ShowKick(false);
            pc.UpdateDisplay();
        }
    }

    private void ClientOnReadyToggled(bool newValue) 
    {
        UpdateReadyServerRpc(newValue);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateReadyServerRpc(bool newValue, ServerRpcParams rpcParams = default)
    {
        networkedPlayers.UpdateReady(rpcParams.Receive.SenderClientId, newValue);
    }
}
