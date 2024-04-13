using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class RoomUIController : NetworkBehaviour
{
    [SerializeField] private RectTransform playersUI;
    [SerializeField] private RectTransform underUI;
    [SerializeField] private GameObject gridObj;

    [SerializeField] private RoomPlayer player;
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

        if (IsClient)
        {
            ComeLobbyServerRpc(NetworkManager.LocalClientId);
            RefreshPlayersUIServerRpc();
        }
    }

    public void GameStart()
    {
        FadeUIClientRpc();

        playersUI.DOLocalMoveX(0, 0.5f).OnComplete(() =>
        {
            HostSingle.Instance.GameManager.ChangeLobbyUpdate(true);
            NetworkManager.SceneManager.LoadScene(SceneList.GameScene, LoadSceneMode.Single);
        });
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

    [ServerRpc]
    private void ComeLobbyServerRpc(ulong clientId)
    {
        RoomPlayer newPlayer = Instantiate(player, Vector2.zero, Quaternion.identity);
        newPlayer.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
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

    [ClientRpc]
    private void FadeUIClientRpc()
    {
        gridObj.SetActive(false);
        playersUI.DOLocalMoveY(300, 0.5f);
        underUI.DOLocalMoveY(-300, 0.5f);
    }

    public override void OnDestroy()
    {
        if (!IsServer) return;

        foreach (var player in FindObjectsOfType<RoomPlayer>())
        {
            player.GetComponent<NetworkObject>().Despawn();
        }

        HostSingle.Instance.NetworkServer.OnClientLeftEvt -= RefreshPlayersUIServerRpc;
    }
}
