using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class ClientSingle : MonoBehaviour
{
    private static ClientSingle instance;

    public ClientGameManager GameManager { get; private set; }

    public static ClientSingle Instance
    {
        get
        {
            if (instance != null) return instance;

            instance = FindObjectOfType<ClientSingle>();

            if (instance == null)
                return null;

            return instance;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CreateClient()
    {
        GameManager = new ClientGameManager(NetworkManager.Singleton);
    }
}
