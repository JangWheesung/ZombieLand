using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : PlayerRoot
{
    [SerializeField] private float speed;

    public override void OnNetworkSpawn()
    {
        playerController.OnMovementEvt += Movement;
    }

    private void Movement(Vector2 vec)
    {
        rb.velocity = vec * speed;
    }
}
