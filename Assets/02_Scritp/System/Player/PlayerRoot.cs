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
    protected PlayerAnimation playerAnimation;

    protected SpriteRenderer sp;
    protected Collider2D col;
    protected Rigidbody2D rb;
    protected Animator anim;

    protected virtual void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAbility = GetComponent<PlayerAbility>();
        playerDetect = GetComponent<PlayerDetect>();
        playerAnimation = GetComponent<PlayerAnimation>();

        sp = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
}
