using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public enum PlayerRole
{
    None,
    Human,
    Zombie
}

public class RoleManager : NetworkBehaviour
{
    public static RoleManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void AssignedToRole()
    {
        //Random.Range(0, NetworkManager.ConnectedClients.Count)
        int zombieIndex = 1;
        foreach (var item in NetworkManager.ConnectedClientsIds)
        {
            PlayerRole newPlayerRole = zombieIndex == 0 ? PlayerRole.Zombie : PlayerRole.Human;
            GameManager.Instance.PlayerRoleChange(item, newPlayerRole);

            zombieIndex--;
        }
    }
}
