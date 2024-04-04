using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //이벤트
    public event Action<Vector2> OnMovementEvt;
    public event Action OnLeftClickEvt;

    //다른 클래스

    //다른 요소

    //참조값

    public void SetPlayerRole() //역할 정하기(좀비or인간)
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
}
