using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text lobbyNameText;
    [SerializeField] private TMP_Text lobbyConnectText;
    [SerializeField] private Button joinBtn;

    private string joinCode;

    private void Start()
    {
        joinBtn.onClick.AddListener(JoinButtonClick);
    }

    public void SetLobbyUIData(Lobby lobby)
    {
        joinCode = lobby.Data["JoinCode"].Value;
        lobbyNameText.text = lobby.Name;
        lobbyConnectText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }

    private async void JoinButtonClick()
    {
        await ApplicationController.Instance.StartClientAsync("Jang_Client", joinCode);
    }
}
