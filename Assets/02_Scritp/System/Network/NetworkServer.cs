using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;

    private Dictionary<ulong, string> clientIdToAuthDictionary = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> authIdToUserDataDictionary = new Dictionary<string, UserData>();

    public event Action OnClientJoinEvt;
    public event Action OnClientLeftEvt;

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
        
        clientIdToAuthDictionary[req.ClientNetworkId] = data.authId;
        authIdToUserDataDictionary[data.authId] = data;

        res.CreatePlayerObject = false; //자동생성 안씀
        res.Approved = true; //응답완료

        OnClientJoinEvt?.Invoke();
    }

    private void ServerStarted()
    {
        networkManager.OnClientDisconnectCallback += ClientDisconnect;
    }

    public UserData? GetUserDataByClientID(ulong clientId)
    {
        if (clientIdToAuthDictionary.TryGetValue(clientId, out string authId))
        {
            if (authIdToUserDataDictionary.TryGetValue(authId, out UserData data))
            {
                return data;
            }
        }

        return null;
    }

    public UserData? GetUserDataByAuthID(string authId)
    {
        if (authIdToUserDataDictionary.TryGetValue(authId, out UserData data))
        {
            return data;
        }

        return null;
    }

    public void SetUserDataByClientId(ulong clientId, UserData newUserData)
    {
        if (clientIdToAuthDictionary.TryGetValue(clientId, out string authId))
        {
            if (authIdToUserDataDictionary.TryGetValue(authId, out UserData data))
            {
                authIdToUserDataDictionary[authId] = newUserData;
            }
        }
    }

    private void ClientDisconnect(ulong clientId)
    {
        if (clientIdToAuthDictionary.TryGetValue(clientId, out string authId))
        {
            clientIdToAuthDictionary.Remove(clientId);
            authIdToUserDataDictionary.Remove(authId);
            OnClientLeftEvt?.Invoke();
        }
    }

    public void Dispose()
    {
        if (networkManager == null) return;

        OnClientJoinEvt = null;
        OnClientLeftEvt = null;

        networkManager.ConnectionApprovalCallback -= ApprovalCheck;
        networkManager.OnServerStarted -= ServerStarted;
        networkManager.OnClientDisconnectCallback -= ClientDisconnect;

        if (networkManager.IsListening)
        {
            networkManager.Shutdown();
        }
    }
}
