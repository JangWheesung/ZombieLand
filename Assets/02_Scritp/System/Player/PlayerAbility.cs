using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerAbility : PlayerRoot
{
    [SerializeField] private int flashBangCount;
    [SerializeField] private float infectionRadius;
    private int currentFlashBang;

    protected override void Awake()
    {
        base.Awake();

        currentFlashBang = flashBangCount;

        playerController.OnLeftClickEvt += UseFlashBang;
        playerController.OnLeftClickEvt += UseInfection;
        if (IsOwner)
        {
            GameManager.Instance.OnRoleChangeOwnerEvt += ResetFlashBang;
        }
    }

    private void ResetFlashBang(PlayerRole role, PlayerController player)
    {
        if (role == PlayerRole.Human)
            currentFlashBang = flashBangCount;
    }

    private void UseFlashBang(bool value)
    {
        if (playerController.playerRole != PlayerRole.Human) return;

        if (currentFlashBang > 0)
        {
            //ÇöÀç ¸¶¿ì½º Ä¿¼­·Î ¼¶±¤Åº ÅõÃ´

            currentFlashBang--;
        }
    }

    private void UseInfection(bool value)
    {
        if (playerController.playerRole != PlayerRole.Zombie) return;

        Collider2D[] players = Physics2D.OverlapCircleAll(transform.position, infectionRadius, LayerMask.GetMask("Player"));
        foreach (var item in players)
        {
            if (item.TryGetComponent<PlayerController>(out PlayerController player) 
                && player.playerRole == PlayerRole.Human)
            {
                InfectionServerRpc(player.OwnerClientId);
            }
        }
    }

    [ServerRpc]
    private void InfectionServerRpc(ulong clientId)
    {
        GameManager.Instance.PlayerRoleChange(clientId, PlayerRole.Zombie);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, infectionRadius);
    }
#endif
}
