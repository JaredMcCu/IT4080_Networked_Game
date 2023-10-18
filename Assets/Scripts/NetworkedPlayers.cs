using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkedPlayers : NetworkBehaviour
{
    public NetworkList<NetworkPlayerInfo> allNetPlayers;

    private void Awake() {
        allNetPlayers = new NetworkList<NetworkPlayerInfo>();
    }
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        if (IsServer)
        {
            ServerStart();
        }
    }

    void ServerStart()
    {
        NetworkManager.OnClientConnectedCallback += ServerOnClientConnected;

        NetworkPlayerInfo info = new NetworkPlayerInfo(NetworkManager.LocalClientId);
        info.ready = true;
        allNetPlayers.Add(info);
    }

    private void ServerOnClientConnected(ulong clientId)
    {
        NetworkPlayerInfo info = new NetworkPlayerInfo(clientId);
        info.ready = false;
        allNetPlayers.Add(info);
    }
}
