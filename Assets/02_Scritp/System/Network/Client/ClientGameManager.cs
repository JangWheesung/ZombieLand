using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager
{
    private JoinAllocation allocation;

    public ClientGameManager(NetworkManager networkManager)
    {
        this.networkManager = networkManager;
    }

    private NetworkManager networkManager;

    public async Task StartClientAsync(string joinCode, UserData userData)
    {
        allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        var relayServerData = new RelayServerData(allocation, "dtls");
        transport.SetRelayServerData(relayServerData);

        string userJson = JsonUtility.ToJson(userData);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.UTF8.GetBytes(userJson);

        NetworkManager.Singleton.StartClient();

        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    private void HandleClientDisconnect(ulong outClientId) //게임 나갈시
    {
        networkManager.OnClientDisconnectCallback -= HandleClientDisconnect;
        Disconnect();
    }

    public void Disconnect() //클라 종료
    {
        if (networkManager.IsConnectedClient)
        {
            networkManager.Shutdown();
        }

        SceneManager.LoadScene(SceneList.IntroScene);
    }
}
