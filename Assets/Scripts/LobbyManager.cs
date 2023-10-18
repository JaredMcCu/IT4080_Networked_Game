using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    public Button startButton;
    public TMPro.TMP_Text statusLabel;

    // Start is called before the first frame update
    void Start()
    {
        startButton.gameObject.SetActive(false);
        statusLabel.text = "Start something, like the server or the host or the client.";

        startButton.onClick.AddListener(OnStartButtonClicked);
        NetworkManager.OnClientStarted += OnClientStarted;
        NetworkManager.OnServerStarted += OnServerStarted;
    }

    private void OnServerStarted() {
        GotoLobby();
        //startButton.gameObject.SetActive(true);
        //statusLabel.text = "Press Start";
    }

    private void OnClientStarted() {
        if (!IsHost) {
            statusLabel.text = "Waiting for the game to start";
        }
    }

    private void OnStartButtonClicked() 
    {
        StartGame();
    }

    public void GotoLobby()
    {
        NetworkManager.SceneManager.LoadScene("Lobby", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void StartGame() 
    {
        NetworkManager.SceneManager.LoadScene("Arena1Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
