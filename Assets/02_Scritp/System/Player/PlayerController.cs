using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class PlayerController : PlayerRoot
{
    //�̺�Ʈ
    public event Action<Vector2> OnMovementEvt;
    public event Action<bool> OnLeftClickEvt;

    //�ٸ� Ŭ����

    //�ٸ� ���

    //������
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

    public void SetPlayerRole() //���� ���ϱ�(����or�ΰ�)
    {

    }

    public override void OnDestroy()
    {
        OnMovementEvt = null;
        OnLeftClickEvt = null;
    }
}
