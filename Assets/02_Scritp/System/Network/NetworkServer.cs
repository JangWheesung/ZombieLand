using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;

    private Dictionary<ulong, string> _clientIdToAuthDictionary = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> _authIdToUserDataDictionary = new Dictionary<string, UserData>();

    public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;
        this.networkManager.ConnectionApprovalCallback += ApprovalCheck;

        this.networkManager.OnServerStarted += ServerStarted;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest req,
        NetworkManager.ConnectionApprovalResponse res)
    {
        string json = Encoding.UTF8.GetString(req.Payload);
        UserData data = JsonUtility.FromJson<UserData>(json);

        _clientIdToAuthDictionary[req.ClientNetworkId] = data.authId;
        _authIdToUserDataDictionary[data.authId] = data;

        res.CreatePlayerObject = true; //자동생성 안씀
        res.Approved = true; //응답완료
    }

    private void ServerStarted()
    {
        networkManager.OnClientDisconnectCallback += ClientDisconnect;
    }

    private void ClientDisconnect(ulong clientId)
    {
        if (_clientIdToAuthDictionary.TryGetValue(clientId, out string authId))
        {
            _clientIdToAuthDictionary.Remove(clientId);
            _authIdToUserDataDictionary.Remove(authId);
        }
    }

    public void Dispose()
    {
        if (networkManager == null) return;

        networkManager.ConnectionApprovalCallback -= ApprovalCheck;
        networkManager.OnServerStarted -= ServerStarted;
        networkManager.OnClientDisconnectCallback -= ClientDisconnect;

        if (networkManager.IsListening)
        {
            networkManager.Shutdown();
        }
    }
}
