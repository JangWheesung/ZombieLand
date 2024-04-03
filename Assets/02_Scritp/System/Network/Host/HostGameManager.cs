using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class HostGameManager : IDisposable
{
    private Allocation allocation;
    private string joinCode;
    public string JoinCode => joinCode;

    private string lobbyId;
    private const int maxConnections = 20;

    private NetworkServer networkServer;
    public NetworkServer NetworkServer => networkServer;

    public async Task StartHostAsync(string lobbyName, UserData userData)
    {
        allocation = await Relay.Instance.CreateAllocationAsync(maxConnections);
        joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        var relayServerData = new RelayServerData(allocation, "dtls");
        transport.SetRelayServerData(relayServerData);

        CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
        lobbyOptions.Data = new Dictionary<string, DataObject>()
        {
            {
                "JoinCode", new DataObject(visibility:DataObject.VisibilityOptions.Public, value:joinCode)
            },

            {
                "UserName", new DataObject(visibility:DataObject.VisibilityOptions.Member, value:userData.nickName)
            }
        };

        Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, maxConnections, lobbyOptions);
        lobbyId = lobby.Id;
        HostSingle.Instance.StartCoroutine(HeartbeatLobby(10));

        string userJson = JsonUtility.ToJson(userData); //데이터 가져오기
        NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.UTF8.GetBytes(userJson);

        if (NetworkManager.Singleton.StartHost()) //호스트 연결 성공시점
        {
            networkServer = new NetworkServer(NetworkManager.Singleton);
        }
    }

    private IEnumerator HeartbeatLobby(int time)
    {
        var timer = new WaitForSecondsRealtime(time);

        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);

            yield return timer;
        }
    }

    public void Dispose()
    {
        Shutdown();
    }

    private async void Shutdown()
    {
        //하트비트 꺼주기
        HostSingle.Instance.StopAllCoroutines();

        if (!string.IsNullOrEmpty(lobbyId))
        {
            await Lobbies.Instance.DeleteLobbyAsync(lobbyId);
        }

        lobbyId = string.Empty;
        networkServer?.Dispose();
    }
}
