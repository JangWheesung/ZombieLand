using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostSingle : MonoBehaviour
{
    private static HostSingle instance;

    public HostGameManager GameManager { get; private set; }

    public static HostSingle Instance
    {
        get
        {
            if (instance != null) return instance;

            instance = FindObjectOfType<HostSingle>();

            if (instance == null)
                return null;

            return instance;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CreateHost()
    {
        GameManager = new HostGameManager();
    }
}
