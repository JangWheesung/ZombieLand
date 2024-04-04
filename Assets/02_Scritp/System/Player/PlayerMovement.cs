using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : PlayerRoot
{
    [SerializeField] private float speed;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        playerController.OnMovementEvt += Movement;
    }

    private void Movement(Vector2 vec)
    {
        rb.velocity = vec * speed;
    }
}
