using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkedPlayers : NetworkBehaviour
{
    public NetworkList<NetworkPlayerInfo> allNetPlayers;

    private int colorIndex = 0;
    private Color[] playerColors = new Color[] {
        Color.green,
        Color.yellow,
        Color.red,
        Color.black,
        Color.gray,
        Color.cyan,
        Color.blue,
    };

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
        info.color = NextColor();
        allNetPlayers.Add(info);
    }

    private void ServerOnClientConnected(ulong clientId)
    {
        NetworkPlayerInfo info = new NetworkPlayerInfo(clientId);
        info.ready = false;
        info.color = NextColor();
        allNetPlayers.Add(info);
    }

    private Color NextColor() {
        Color newColor = playerColors[colorIndex];
        colorIndex += 1;
        if(colorIndex > playerColors.Length -1) {
            colorIndex = 0;
        }
        return newColor;
    }

    public int FindPlayerIndex(ulong clientId) {
    var idx = 0;
    var found = false;

    while (idx < allNetPlayers.Count && !found) {
        if (allNetPlayers[idx].clientId == clientId) {
            found = true;
        } else {
            idx += 1;
        }
    }

    if (!found) {
        idx = -1;
    }

    return idx;
    }     

    public void UpdateReady(ulong clientId, bool ready)
    {
        int idx = FindPlayerIndex(clientId);
        if(idx == -1) {
            return;
        }

        NetworkPlayerInfo info = allNetPlayers[idx];
        info.ready = ready;
        allNetPlayers[idx] = info;
    }       
}
