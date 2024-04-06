using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ModelManager : NetworkBehaviour
{
    public static ModelManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (IsClient)
        {
            GameManager.Instance.OnRoleChangeEvt += SetPlayerSprite;
        }
    }

    private void SetPlayerSprite(PlayerRole role, PlayerController player)
    {
        if (role == PlayerRole.Human)
        {
            //���߿� ��������Ʈ �ٲ� �ٰ���
            player.PlayerColorChange(Color.blue);
        }
        else if (role == PlayerRole.Zombie)
        {
            player.PlayerColorChange(Color.red);
        }
    }
}
