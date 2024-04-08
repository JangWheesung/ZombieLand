using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ParticleLife : NetworkBehaviour
{
    private ParticleSystem particle;
    [SerializeField] private float lifeTime;
    [SerializeField] private bool isLifeSpan;

    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
        particle.Play();
        if (isLifeSpan)
        {
            StartCoroutine(LifeCount());
        }
    }

    public void IsDeathParticle()
    {
        Destroy(gameObject);
    }

    private IEnumerator LifeCount()
    {
        yield return new WaitForSecondsRealtime(lifeTime);
        Destroy(gameObject);
    }
}