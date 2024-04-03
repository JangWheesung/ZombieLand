using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private HostSingle hostSingle;
    [SerializeField] private ClientSingle clientSingle;

    public static ApplicationController Instance;

    private void Awake()
    {
        Instance = this;
    }

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await UnityServices.InitializeAsync();

        var state = await AuthenticationWrapper.DoAuth();

        if (state != AuthState.Authenticated)
        {
            Debug.Log("인증 실패");
            return;
        }

        HostSingle host = Instantiate(hostSingle, transform);
        host.CreateHost();

        ClientSingle client = Instantiate(clientSingle, transform);
        client.CreateClient();

#if UNITY_EDITOR
        SceneManager.LoadScene(SceneList.IntroScene);
#endif
    }
}
