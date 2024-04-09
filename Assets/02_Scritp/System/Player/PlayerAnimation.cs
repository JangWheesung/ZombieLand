using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerAnimation : PlayerRoot
{
    public void OnMoveAnim(bool value)
    {
        OnMoveAnimServerRpc(OwnerClientId, value);
    }

    public void OnAttackAnim()
    {
        OnAttackAnimServerRpc(OwnerClientId);
    }

    public void OnFilp(bool isLeft)
    {
        OnFilpServerRpc(OwnerClientId, isLeft);
    }

    #region ServerRpc

    [ServerRpc]
    private void OnMoveAnimServerRpc(ulong clientId, bool value)
    {
        OnMoveAnimClientRpc(clientId, value);
    }

    [ServerRpc]
    private void OnAttackAnimServerRpc(ulong cliendId)
    {
        OnAttackAnimClientRpc(cliendId);
    }

    [ServerRpc]
    private void OnFilpServerRpc(ulong clientId, bool isLeft)
    {
        OnFilpClientRpc(clientId, isLeft);
    }

    #endregion

    #region ClientRpc

    [ClientRpc]
    private void OnMoveAnimClientRpc(ulong clientId, bool value)
    {
        if (clientId != OwnerClientId) return;

        anim.SetBool("IsWalk", value);
    }

    [ClientRpc]
    private void OnAttackAnimClientRpc(ulong clientId)
    {
        if (clientId != OwnerClientId) return;

        anim.SetTrigger("Attack");
    }

    [ClientRpc]
    private void OnFilpClientRpc(ulong clientId, bool isLeft)
    {
        if (clientId != OwnerClientId) return;

        sp.flipX = isLeft;
    }

    #endregion
}
