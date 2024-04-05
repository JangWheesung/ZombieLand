using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerRoot : NetworkBehaviour
{
    protected PlayerController playerController;
    protected PlayerMovement playerMovement;

    [HideInInspector] public SpriteRenderer sp;
    [HideInInspector] public Collider2D col;
    [HideInInspector] public Rigidbody2D rb;

    protected virtual void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerMovement = GetComponent<PlayerMovement>();
        sp = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }
}
