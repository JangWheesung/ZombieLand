using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerDetect : PlayerRoot
{
    [SerializeField] private float humanRader;
    [SerializeField] private float zombieRader;

    private void Update()
    {
        if (!IsOwner) return;

        if (playerController.playerRole == PlayerRole.Human)
        {
            Detect(humanRader, PlayerRole.Zombie);
        }
        else if (playerController.playerRole == PlayerRole.Zombie)
        {
            Detect(zombieRader, PlayerRole.Human);
        }
    }

    private void Detect(float rader, PlayerRole role)
    {
        Collider2D[] players = Physics2D.OverlapCircleAll(transform.position, rader, LayerMask.GetMask("Player"));
        foreach (var item in players)
        {
            if (item.TryGetComponent<PlayerController>(out PlayerController player)
                && player.playerRole == role)
            {
                switch (role)
                {
                    case PlayerRole.Human:
                        ZombieVolume();
                        break;
                    case PlayerRole.Zombie:
                        HumanVolume();
                        break;
                }
                return;
            }
        }
        VolumeAssign();
    }

    private void HumanVolume()
    {
        VolumeManager.Instance.BloomIntensity(12f);
        VolumeManager.Instance.FilmGrainIntensity(0.2f);
        VolumeManager.Instance.MotionBlurIntensity(0.2f);
        VolumeManager.Instance.VignetteColor(Color.black);
        VolumeManager.Instance.VignetteIntensity(0.4f);
    }

    private void ZombieVolume()
    {
        VolumeManager.Instance.BloomIntensity(12f);
        VolumeManager.Instance.MotionBlurIntensity(0.2f);
        VolumeManager.Instance.VignetteColor(Color.red);
        VolumeManager.Instance.VignetteIntensity(0.3f);
    }

    private void VolumeAssign()
    {
        VolumeManager.Instance.BloomIntensity(8f);
        VolumeManager.Instance.FilmGrainIntensity(0f);
        VolumeManager.Instance.MotionBlurIntensity(0f);
        VolumeManager.Instance.VignetteIntensity(0f);
    }
}
