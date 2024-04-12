using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class WinUIController : NetworkBehaviour
{
    [SerializeField] private Transform layer;
    [SerializeField] private PlayerWinUI playerUI;

    private void Start()
    {
        if (!IsServer) return;

        foreach (var data in NetworkManager.ConnectedClientsIds)
        {
            CreatePlayerPanelClientRpc(HostSingle.Instance.NetworkServer.GetUserDataByClientID(data).Value);
        }
    }

    [ClientRpc]
    private void CreatePlayerPanelClientRpc(UserData data)
    {
        PlayerWinUI uI = Instantiate(playerUI, layer);
        uI.SetData(data);
    }

    public void BackBtn()
    {
        if (IsHost)
        {
            HostSingle.Instance.NetworkServer.Dispose();
            NetworkManager.Singleton.SceneManager.LoadScene(SceneList.IntroScene, LoadSceneMode.Single);
        }
        else if (IsClient)
        {
            ClientSingle.Instance.GameManager.Disconnect();
        }
    }
}
