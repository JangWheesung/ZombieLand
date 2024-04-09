using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using DG.Tweening;

public class PlayerAbility : PlayerRoot
{
    [SerializeField] private FlashBang flashBangPrefab;

    [SerializeField] private GameObject infectionCircle;
    [SerializeField] private GameObject arrowRoot;

    [SerializeField] private int flashBangCount;
    [SerializeField] private float drowPower;
    [SerializeField] private float infectionRadius;

    private int currentFlashBang;

    public override void OnNetworkSpawn()
    {
        infectionCircle.transform.localScale = new Vector2(infectionRadius * 2, infectionRadius * 2);

        playerController.OnLeftClickEvt += UseFlashBang;
        playerController.OnLeftClickEvt += UseInfection;
        if (IsOwner)
        {
            GameManager.Instance.OnRoleChangeOwnerEvt += ResetFlashBang;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (!arrowRoot.activeSelf) return;

        Vector2 vec = CameraManager.Instance.MouseVecter2D() - (Vector2)transform.position;
        arrowRoot.transform.up = vec.normalized;
    }

    private void ResetFlashBang(PlayerRole role, PlayerController player)
    {
        if (role == PlayerRole.Human)
            ResetFlashBangServerRpc();
    }

    private void UseFlashBang(bool value)
    {
        if (playerController.playerRole != PlayerRole.Human) return;

        if (value)
        {
            arrowRoot.SetActive(true);
        }
        else
        {
            playerAnimation.OnAttackAnim();

            arrowRoot.SetActive(false);
            UseFlashBangServerRpc();
        }
    }

    private void UseInfection(bool value)
    {
        if (playerController.playerRole != PlayerRole.Zombie) return;

        if (value)
        {
            infectionCircle.SetActive(true);
        }
        else
        {
            playerAnimation.OnAttackAnim();

            infectionCircle.SetActive(false);
            Collider2D[] players = Physics2D.OverlapCircleAll(transform.position, infectionRadius, LayerMask.GetMask("Player"));
            foreach (var item in players)
            {
                if (item.TryGetComponent<PlayerController>(out PlayerController player)
                    && player.playerRole == PlayerRole.Human)
                {
                    GameManager.Instance.PlayerRoleChange(player.OwnerClientId, PlayerRole.Zombie);
                }
            }
            CameraManager.Instance.ShakeCamera(OwnerClientId, 2, 0.8f, 0.15f);
        }
    }

    #region ServerRpc

    [ServerRpc]
    private void UseFlashBangServerRpc()
    {
        if (currentFlashBang > 0)
        {
            Vector2 vec = CameraManager.Instance.MouseVecter2D() - (Vector2)transform.position;

            FlashBang grenade = Instantiate(flashBangPrefab, transform.position, Quaternion.identity);
            grenade.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
            grenade.Drow(vec.normalized, drowPower);

            currentFlashBang--;
        }
    }

    [ServerRpc]
    private void ResetFlashBangServerRpc()
    {
        currentFlashBang = flashBangCount;
    }

    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, infectionRadius);
    }
#endif
}
