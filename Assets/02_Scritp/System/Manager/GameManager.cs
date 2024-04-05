using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Threading.Tasks;
using System.Linq;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    //클라만 받는 이벤트
    public event Action<PlayerRole, PlayerController> OnRoleChangeEvt;
    public event Action<PlayerRole, PlayerController> OnRoleChangeOwnerEvt;

    [SerializeField] private PlayerController playerPrefab;
    [SerializeField] private List<Transform> spawnTrs;

    private List<PlayerController> players = new List<PlayerController>();
    private List<PlayerController> humanPlayers = new List<PlayerController>();
    private List<PlayerController> zombiePlayers = new List<PlayerController>();

    public PlayerController localPlayerController;

    private void Awake()
    {
        Instance = this;
    }

    private async void Start()
    {
        await Task.Delay(5000);

        if (!IsServer) return;

        SpawnPlayer();
    }

    //public override void OnNetworkSpawn() //Start보다 늦게 실행
    //{
    //    if (!IsServer) return;

    //    SpawnPlayer();
    //}

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

    public void PlayerRoleChange(ulong clientId, PlayerRole role)
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
        else if(role == PlayerRole.Zombie)
        {
            humanPlayers.Remove(player);
            zombiePlayers.Add(player);
        }
    }

    public void PlayerLeft(ulong clientId)
    {
        PlayerLeftServerRpc(clientId);
    }

    private void SpawnPlayer()
    {
        foreach (var player in NetworkManager.ConnectedClientsIds)
        {
            int randomOrder = UnityEngine.Random.Range(0, spawnTrs.Count);

            PlayerController newPlayer = Instantiate(playerPrefab, spawnTrs[randomOrder].position, Quaternion.identity);
            newPlayer.GetComponent<NetworkObject>().SpawnWithOwnership(player);

            players.Add(newPlayer);

            spawnTrs.Remove(spawnTrs[randomOrder]);
        }

        RoleManager.Instance.AssignedToRole();
    }

    [ClientRpc]
    private void PlayerRoleChangeClientRpc(ulong clientId, UserData data)
    {
        //모든 클라에 해당 플레이어 가지고 온다
        var player = FindObjectsOfType<PlayerController>().ToList().Find(x => x.OwnerClientId == clientId);
        OnRoleChangeEvt?.Invoke(data.playerRole, player);

        if (clientId != NetworkManager.LocalClientId) return;
        OnRoleChangeOwnerEvt?.Invoke(data.playerRole, player);
    }


    [ServerRpc]
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
}
