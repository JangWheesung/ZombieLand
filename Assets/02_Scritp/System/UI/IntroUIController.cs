using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;

public class IntroUIController : MonoBehaviour
{
    [SerializeField] private LobbyUI lobbyUIPrefab;
    [SerializeField] private RectTransform lobbyPanelRoot;

    private void Start()
    {
        StartCoroutine(RefreshLobby());
    }

    public async void CreateRoom(TMP_InputField roomInputField)
    {
        await ApplicationController.Instance.StartHostAsync("Jang_Host", roomInputField.text);
        NetworkManager.Singleton.SceneManager.LoadScene(SceneList.roomScene, LoadSceneMode.Single);
    }

    public async void JoinRoom(TMP_InputField joinInputField)
    {
        await ApplicationController.Instance.StartClientAsync("Jang_Client", joinInputField.text);
    }

    private void CreateLobbyUI(Lobby lobby)
    {
        LobbyUI ui = Instantiate(lobbyUIPrefab, lobbyPanelRoot);
        ui.SetLobbyUIData(lobby);
    }

    private IEnumerator RefreshLobby()
    {
        while (true)
        {
            var lobbys = ApplicationController.Instance.GetLobbyList();

            yield return new WaitUntil(() => lobbys.IsCompleted);

            var childs = lobbyPanelRoot.GetComponentsInChildren<LobbyUI>();

            foreach (var child in childs)
            {
                Destroy(child.gameObject);
            }

            foreach (var lobby in lobbys.Result)
            {
                CreateLobbyUI(lobby);
            }

            yield return new WaitForSeconds(1f); //1초마다 패널 리프레쉬
        }
    }
}
