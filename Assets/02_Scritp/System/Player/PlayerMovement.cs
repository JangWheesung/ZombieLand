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
        playerAnimation.OnMoveAnim(vec != Vector2.zero);
        if (vec.x != 0)
        {
            playerAnimation.OnFilp(vec.x < 0);
        }

        rb.velocity = vec * speed;
    }
}
