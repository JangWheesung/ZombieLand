using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Threading.Tasks;
using System.Linq;
using DG.Tweening;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    //이벤트는 클라에서 달아준다.
    public event Action OnGameStartEvt;
    public event Action OnPlayerSpawnEndEvt;
    public event Action<PlayerRole, PlayerController> OnRoleChangeEvt;
    public event Action<PlayerRole, PlayerController> OnRoleChangeOwnerEvt;

    [SerializeField] private GeneratorManager generatorManagerPrefab;
    [SerializeField] private PlayerRoot playerPrefab;
    [SerializeField] private List<Transform> spawnTrs;

    [SerializeField] private float settingTime;

    private List<PlayerController> players = new List<PlayerController>();
    private List<PlayerController> humanPlayers = new List<PlayerController>();
    private List<PlayerController> zombiePlayers = new List<PlayerController>();

    public PlayerController localPlayerController;

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        if (IsServer)
        {
            MapSetting();
        }

        Camera.main.DOOrthoSize(30f, settingTime / 2f).SetEase(Ease.OutExpo);
        yield return new WaitForSeconds(settingTime); //시작 시 연출

        OnGameStartEvt?.Invoke();

        if (IsServer)
        {
            SpawnPlayer();
        }
    }

    private void MapSetting()
    {
        MapManager.Instance.CreateMap();

        for (int i = 0; i < spawnTrs.Count; i++)
        {
            spawnTrs[i].position = MapManager.Instance.GetRandomPointInFloorMap();
        }
    }

    private void SpawnPlayer()
    {
        foreach (var player in NetworkManager.ConnectedClientsIds)
        {
            int randomOrder = UnityEngine.Random.Range(0, spawnTrs.Count);

            PlayerRoot newPlayer = Instantiate(playerPrefab, spawnTrs[randomOrder].position, Quaternion.identity);
            newPlayer.GetComponent<NetworkObject>().SpawnWithOwnership(player);

            players.Add(newPlayer.GetComponent<PlayerController>());

            spawnTrs.Remove(spawnTrs[randomOrder]);
        }

        RoleManager.Instance.AssignedToRole();

        SpawnEndClientRpc();
    }

    #region ServerComponent

    public void SetLocalPlayerController(PlayerController playerController)
    {
        localPlayerController = playerController;
    }

    public PlayerController GetPlayerControllerByClientID(ulong clientId)
    {
        return players.Find(x => x.OwnerClientId == clientId);
    }

    public int GetPlayersCount(PlayerRole playerRole = PlayerRole.None)
    {
        switch (playerRole)
        {
            case PlayerRole.None:
                return players.Count;
            case PlayerRole.Human:
                return humanPlayers.Count;
            case PlayerRole.Zombie:
                return zombiePlayers.Count;
        }
        return 0;
    }

    #endregion

    #region ClientComponent

    public void PlayerRoleChange(ulong clientId, PlayerRole role)
    {
        PlayerRoleChangeServerRpc(clientId, role);
    }

    public void PlayerLeft(ulong clientId)
    {
        PlayerLeftServerRpc(clientId);
    }

    #endregion

    #region ServerRpc

    [ServerRpc(RequireOwnership = false)]
    private void PlayerRoleChangeServerRpc(ulong clientId, PlayerRole role)
    {
        var data = HostSingle.Instance.NetworkServer.GetUserDataByClientID(clientId).Value;
        data.playerRole = role;
        HostSingle.Instance.NetworkServer.SetUserDataByClientId(clientId, data);
        PlayerRoleChangeClientRpc(clientId, data);

        var player = GetPlayerControllerByClientID(clientId);

        if (role == PlayerRole.Human)
        {
            zombiePlayers.Remove(player);
            humanPlayers.Add(player);
        }
        else if (role == PlayerRole.Zombie)
        {
            humanPlayers.Remove(player);
            zombiePlayers.Add(player);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayerLeftServerRpc(ulong clientId)
    {
        PlayerController player = GetPlayerControllerByClientID(clientId);

        if (humanPlayers.Equals(player))
        {
            humanPlayers.Remove(player);
        }
        if (zombiePlayers.Equals(player))
        {
            zombiePlayers.Remove(player);
        }
        players.Remove(player);
    }

    #endregion

    #region ClientRpc

    [ClientRpc]
    private void SpawnEndClientRpc()
    {
        OnPlayerSpawnEndEvt?.Invoke();
    }

    [ClientRpc]
    private void PlayerRoleChangeClientRpc(ulong clientId, UserData data)
    {
        //모든 클라에 해당 플레이어 가지고 온다
        var player = FindObjectsOfType<PlayerController>().ToList().Find(x => x.OwnerClientId == clientId);
        player.PlayerRoleChange(data.playerRole);

        OnRoleChangeEvt?.Invoke(data.playerRole, player);
        if (clientId == NetworkManager.LocalClientId)
        {
            OnRoleChangeOwnerEvt?.Invoke(data.playerRole, player);
        }
    }

    #endregion

    public override void OnDestroy()
    {
        foreach (PlayerController player in players)
        {
            player.GetComponent<NetworkObject>().Despawn();
        }
    }
}
