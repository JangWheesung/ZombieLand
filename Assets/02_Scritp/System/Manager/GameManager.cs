using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private Transform[] spawnTrs;

    public static GameManager Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        
    }
}
