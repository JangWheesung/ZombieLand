using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerController : PlayerRoot
{
    public event Action<Vector2> OnMovementEvt;
    public event Action<bool> OnLeftClickEvt;

    [SerializeField] private InputReader inputReader;
    [SerializeField] private CinemachineVirtualCamera vcam;

    public PlayerRole playerRole;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (IsOwner)
        {
            inputReader.MovementEvent += HandleMove;
            inputReader.LeftClickEvent += HandleLClick;

            GameManager.Instance.SetLocalPlayerController(this);
            CameraManager.Instance.CinemachSetting(vcam);
        }
        else
        {
            Destroy(vcam);
        }
    }

    public void PlayerRoleChange(PlayerRole role)
    {
        playerRole = role;
    }

    public void PlayerColorChange(Color color)
    {
        sp.color = color;
    }

    private void HandleMove(Vector2 value)
    {
        OnMovementEvt?.Invoke(value);
    }

    private void HandleLClick(bool value)
    {
        OnLeftClickEvt?.Invoke(value);
    }

    public override void OnNetworkDespawn()
    {
        OnMovementEvt = null;
        OnLeftClickEvt = null;

        if (!IsOwner) return;
        GameManager.Instance.PlayerLeft(OwnerClientId);
    }
}
