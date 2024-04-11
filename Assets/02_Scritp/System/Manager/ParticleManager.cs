using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ParticleManager : NetworkBehaviour
{
    public static ParticleManager Instance;

    [SerializeField] private ParticleLife bombParticlePrefab;
    [SerializeField] private ParticleLife zombieParticlePrefab;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.OnRoleChangeEvt += SpawnZombieParticleSpawn;
        //GameManager.Instance.OnPlayerSpawnEndEvt += () =>
        //{
        //    GameManager.Instance.OnRoleChangeEvt += SpawnZombieParticleSpawn;
        //};
    }

    public void SpawnBombParticleSpawn(Vector2 pos)
    {
        SpawnBombParticleSpawnServerRpc(pos);
    }
    public void SpawnZombieParticleSpawn(Vector2 pos)
    {
        SpawnZombieParticleSpawnServerRpc(pos);
    }

    private void SpawnZombieParticleSpawn(PlayerRole role, PlayerController player)
    {
        if (role != PlayerRole.Zombie) return;

        Vector2 pos = player.transform.position;
        SpawnZombieParticleSpawnServerRpc(pos);
    }

    #region ServerRpc

    [ServerRpc(RequireOwnership = false)]
    private void SpawnBombParticleSpawnServerRpc(Vector2 pos)
    {
        ParticleLife bombParticle = Instantiate(bombParticlePrefab, pos, Quaternion.identity);
        bombParticle.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnZombieParticleSpawnServerRpc(Vector2 pos)
    {
        ParticleLife bombParticle = Instantiate(zombieParticlePrefab, pos, Quaternion.identity);
        bombParticle.GetComponent<NetworkObject>().Spawn();
    }

    #endregion
}
