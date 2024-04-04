using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private PlayerController playerPrefab;
    [SerializeField] private List<Transform> spawnTrs;

    public static GameManager Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (!IsServer) return;

        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        foreach (var player in NetworkManager.ConnectedClientsIds)
        {
            int randomOrder = Random.Range(0, spawnTrs.Count);

            PlayerController newPlayer = Instantiate(playerPrefab, spawnTrs[randomOrder].position, Quaternion.identity);
            newPlayer.GetComponent<NetworkObject>().SpawnWithOwnership(player);

            spawnTrs.Remove(spawnTrs[randomOrder]);
        }
    }
}
