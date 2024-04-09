using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerRoot : NetworkBehaviour
{
    protected PlayerController playerController;
    protected PlayerMovement playerMovement;
    protected PlayerAbility playerAbility;
    protected PlayerDetect playerDetect;

    protected SpriteRenderer sp;
    protected Collider2D col;
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAbility = GetComponent<PlayerAbility>();
        playerDetect = GetComponent<PlayerDetect>();

        sp = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }
}
