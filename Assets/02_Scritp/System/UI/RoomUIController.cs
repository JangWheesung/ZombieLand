using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class RoomUIController : NetworkBehaviour
{
    [SerializeField] private PlayerUI playerUIPrefab;
    [SerializeField] private RectTransform playersPanelRoot;
    [SerializeField] private GameObject startBtnObj;

    private void Start()
    {
        if (IsHost) //호스트만 실행되게
        {
            HostSingle.Instance.NetworkServer.OnClientLeftEvt += RefreshPlayersUIServerRpc;
        }
        else
        {
            startBtnObj.SetActive(false);
        }

        RefreshPlayersUIServerRpc();//클라는 호스트 화면의 클라가 실행
    }

    [ServerRpc(RequireOwnership = false)]
    private void RefreshPlayersUIServerRpc()
    {
        var playerUIs = playersPanelRoot.GetComponentsInChildren<PlayerUI>();
        foreach (var playerUI in playerUIs)
        {
            Destroy(playerUI.gameObject);
        }

        foreach (var item in NetworkManager.ConnectedClientsIds)
        {
            Debug.Log(item);
            var data = HostSingle.Instance.GameManager.NetworkServer.GetUserDataByClientID(item);

            if (data != null)
            {
                RefreshPlayersUIClientRpc(data.Value);
            }
        }
    }

    [ClientRpc]
    private void RefreshPlayersUIClientRpc(UserData data)
    {
        PlayerUI ui = Instantiate(playerUIPrefab, playersPanelRoot);
        ui.SetPlayerUIData(data);
    }

    public void GameStart()
    {
        HostSingle.Instance.GameManager.ChangeLobbyUpdate(true);
        NetworkManager.SceneManager.LoadScene(SceneList.GameScene, LoadSceneMode.Single);
    }

    public void BackLobby()
    {
        if (IsHost)
        {
            HostSingle.Instance.GameManager.Dispose();
            SceneManager.LoadScene(SceneList.IntroScene);
        }

        if (IsClient)
        {
            ClientSingle.Instance.GameManager.Disconnect();
        }
    }
}
