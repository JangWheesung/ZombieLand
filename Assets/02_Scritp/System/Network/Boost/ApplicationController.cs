using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;

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

        SceneManager.LoadScene(SceneList.IntroScene);
#if UNITY_EDITOR
        //SceneManager.LoadScene(SceneList.IntroScene);
#endif
    }

    public async Task StartHostAsync(string username, string lobbyName)
    {
        await HostSingle.Instance.GameManager.StartHostAsync(lobbyName, GetUserData(username));
    }

    public async Task StartClientAsync(string username, string joinCode)
    {
        await ClientSingle.Instance.GameManager.StartClientAsync(joinCode, GetUserData(username));
    }

    private UserData GetUserData(string userName)
    {
        return new UserData
        {
            nickName = userName,
            authId = AuthenticationService.Instance.PlayerId
        };
    }

    public async Task<List<Lobby>> GetLobbyList()
    {
        QueryLobbiesOptions options = new QueryLobbiesOptions();
        options.Count = 20;
        options.Filters = new List<QueryFilter>()
        {
            new QueryFilter(field:QueryFilter.FieldOptions.AvailableSlots, op:QueryFilter.OpOptions.GT, value:"0"), //남는 방 부르고
            new QueryFilter(field:QueryFilter.FieldOptions.IsLocked, op:QueryFilter.OpOptions.EQ, value:"0"), //잠겨있지는 않게
        };

        QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);
        return lobbies.Results;
    }
}
