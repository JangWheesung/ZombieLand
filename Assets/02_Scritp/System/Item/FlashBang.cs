using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FlashBang : NetworkBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float delay;
    [SerializeField] private float flashRadius;
    [SerializeField] private float shakeRadius;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        StartCoroutine(Bomb());
    }

    public void Drow(Vector2 vec, float power)
    {
        rb.AddForce(vec * power, ForceMode2D.Impulse);
    }

    public override void OnNetworkDespawn() { }

    private IEnumerator Bomb()
    {
        yield return new WaitForSecondsRealtime(delay);
        
        Collider2D[] flashPlayers = Physics2D.OverlapCircleAll(transform.position, flashRadius, LayerMask.GetMask("Player"));
        foreach (var item in flashPlayers)
        {
            if (item.TryGetComponent<PlayerController>(out PlayerController player))
            {
                float destance = Vector2.Distance(item.transform.position, transform.position);
                Debug.Log(destance);
                CameraManager.Instance.FlashEffect(player.OwnerClientId, FlashLerp(destance));
            }
        }

        Collider2D[] neerPlayers = Physics2D.OverlapCircleAll(transform.position, shakeRadius, LayerMask.GetMask("Player"));
        foreach (var item in neerPlayers)
        {
            if (item.TryGetComponent<PlayerController>(out PlayerController player))
            {
                CameraManager.Instance.ShakeCanera(player.OwnerClientId, 4, 1.3f, 0.2f);
            }
        }

        Destroy(gameObject);
    }

    private float FlashLerp(float value) //OutExpo
    {
        float clampedValue = Mathf.Clamp01(value / flashRadius); //value : 0 ~ 효과범위(구조상 value가 radius를 넘을 수 없음)
        float lerp = -Mathf.Pow(2, -10 * (1 - clampedValue)) + 1;
        Debug.Log(lerp * flashRadius);
        return lerp * flashRadius;
    }

    public override void OnDestroy()
    {
        if (!IsServer) return;

        ParticleManager.Instance.SpawnBombParticleSpawn(transform.position);
    }
}
