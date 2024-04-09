using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using DG.Tweening;

public class FlashBang : NetworkBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float delay;
    [SerializeField] private float flashRadius;
    [SerializeField] private float shakeRadius;

    private float drawPower;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        StartCoroutine(Bomb());
    }

    private void Update()
    {
        if (!IsServer) return;

        if (rb.velocity != Vector2.zero)
        {
            float interpolation = (drawPower * delay) / 6f;
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, interpolation * Time.deltaTime);
        }
    }

    public void Drow(Vector2 vec, float power)
    {
        //transform.DOMove(transform.position + (Vector3)vec * power, delay).SetEase(Ease.OutQuart);
        drawPower = power;
        rb.AddForce(vec * power, ForceMode2D.Impulse);
    }

    public override void OnNetworkDespawn() { }

    private IEnumerator Bomb()
    {
        yield return new WaitForSecondsRealtime(delay);
        
        Collider2D[] flashPlayers = Physics2D.OverlapCircleAll(transform.position, flashRadius, LayerMask.GetMask("Player"));
        foreach (var item in flashPlayers)
        {
            if (item.TryGetComponent(out PlayerController player) && player.OwnerClientId != OwnerClientId)
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
                CameraManager.Instance.ShakeCamera(player.OwnerClientId, 4, 1.5f, 0.3f);
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
