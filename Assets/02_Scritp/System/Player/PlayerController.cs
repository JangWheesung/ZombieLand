using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerController : PlayerRoot
{
    //이벤트
    public event Action<Vector2> OnMovementEvt;
    public event Action<bool> OnLeftClickEvt;

    //다른 클래스

    //다른 요소

    //참조값
    [SerializeField] private InputReader inputReader;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (!IsOwner) return;

        inputReader.MovementEvent += HandleMove;
        inputReader.LeftClickEvent += HandleLClick;
    }

    private void HandleMove(Vector2 value)
    {
        OnMovementEvt?.Invoke(value);
    }

    private void HandleLClick(bool value)
    {
        OnLeftClickEvt?.Invoke(value);
    }

    public void SetPlayerRole() //역할 정하기(좀비or인간)
    {

    }

    public override void OnDestroy()
    {
        OnMovementEvt = null;
        OnLeftClickEvt = null;
    }
}
