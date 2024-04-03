using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class ClientGameManager : MonoBehaviour
{
    public ClientGameManager(NetworkManager networkManager)
    {
        this.networkManager = networkManager;
    }

    private NetworkManager networkManager;

    public async Task StartClientAsync()
    {

    }
}
