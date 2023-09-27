using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class NetworkHelper : MonoBehaviour
{
    private static NetworkManager netMgr = NetworkManager.Singleton;
    private static void StartButtons() {
        if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
    }

    private static void RunningControls() {
        string transportTypeName = NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name;
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        string serverPort = "?";
        if (transport != null) {
            serverPort = $"{transport.ConnectionData.Address}:{transport.ConnectionData.Port}";
        }
        
        string mode = GetNetworkMode();
        if (GUILayout.Button($"Shutdown {mode}")) {
            netMgr.Shutdown();
        }
        GUILayout.Label($"Transport: {transportTypeName} [{serverPort}]");
        GUILayout.Label("Mode: {mode}");
        if (netMgr.IsClient) {
            GUILayout.Label($"ClientId = {netMgr.LocalClientId}");
        }
    }
    public static void GUILayoutNetworkControls() {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer) {
            StartButtons();
        } else {
            RunningControls();
        }
        GUILayout.EndArea();
    }

    public static string GetNetworkMode() {
        string type = "nothing";
        if (netMgr.IsServer) {
            if (netMgr.IsHost) {
                type = "host";
            } else {
                type = "server";
            }
        } else if(netMgr.IsClient) {
            type = "client";
        }
        return type;
    }

    public static void Log(string msg) {
        Debug.Log($"[{GetNetworkMode()} {netMgr.LocalClientId}]:  {msg}");
    }
    public static void Log(NetworkBehaviour what, string msg)
    {
        ulong ownerId = what.GetComponent<NetworkBehaviour>().OwnerClientId;
        Debug.Log($"[{GetNetworkMode()} {netMgr.LocalClientId}][{what.GetType().Name} {ownerId}]:  {msg}");
    }
}
