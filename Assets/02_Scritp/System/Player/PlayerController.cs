using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    //�̺�Ʈ
    public event Action<Vector2> OnMovementEvt;
    public event Action OnLeftClickEvt;

    //�ٸ� Ŭ����

    //�ٸ� ���

    //������

    public void SetPlayerRole() //���� ���ϱ�(����or�ΰ�)
    {

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 moveVecter = context.ReadValue<Vector2>();
        OnMovementEvt?.Invoke(moveVecter);
    }

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        OnLeftClickEvt?.Invoke();
    }

    public override void OnDestroy()
    {
        OnMovementEvt = null;
        OnLeftClickEvt = null;
    }
}
