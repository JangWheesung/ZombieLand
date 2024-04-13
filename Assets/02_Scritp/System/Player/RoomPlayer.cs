using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RoomPlayer : NetworkBehaviour
{
    [SerializeField] private InputReader input;
    [SerializeField] private float speed;

    private PlayerAnimation playerAnimation;
    private Rigidbody2D rb;

    private void Start()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
        rb = GetComponent<Rigidbody2D>();

        input.MovementEvent += Movement;
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

    public override void OnNetworkDespawn()
    {
        input.MovementEvent -= Movement;
    }
}
