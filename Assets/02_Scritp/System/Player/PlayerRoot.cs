using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerRoot : NetworkBehaviour
{
    protected PlayerController playerController;
    protected PlayerMovement playerMovement;

    protected Collider2D col;
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerMovement = GetComponent<PlayerMovement>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }
}
