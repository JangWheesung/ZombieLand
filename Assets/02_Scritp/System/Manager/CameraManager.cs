using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using UnityEngine.UI;
using DG.Tweening;

public class CameraManager : NetworkBehaviour
{
    public static CameraManager Instance;

    [SerializeField] private Image flashImage;

    private CinemachineVirtualCamera vcam;
    private CinemachineBasicMultiChannelPerlin vcamNoise;

    private Camera cam;

    private const int flashProportion = 5;

    private void Awake()
    {
        Instance = this;
        
        cam = Camera.main;
    }

    private void Start()
    {
        GameManager.Instance.OnPlayerSpawnEndEvt += () =>
        {
            if (IsClient)
            {
                GameManager.Instance.OnRoleChangeOwnerEvt += ShakeCameraCor;
            }
        };
    }

    public void CinemachSetting(CinemachineVirtualCamera vcam)
    {
        this.vcam = vcam;
        vcamNoise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public Vector2 MouseVecter2D()
    {
        return cam.ScreenToWorldPoint(Input.mousePosition);
    }

    public void FlashEffect(ulong clientId, float destance)
    {
        FlashEffectServerRpc(clientId, destance);
    }

    public void ShakeCanera(ulong clientId, float amplitude, float freauency, float time)
    {
        ShakeCaneraServerRpc(clientId, amplitude, freauency, time);
    }

    private void ShakeCameraCor(PlayerRole role, PlayerController player)
    {
        StopAllCoroutines();
        if (role == PlayerRole.Zombie) //좀비로 변했으니까
        {
            StartCoroutine(ShakeEffect(5, 2f, 0.4f));
        }
        else if (role == PlayerRole.Human)
        {
            StartCoroutine(ShakeEffect(3, 0.9f, 0.3f));
        }
    }

    #region ServerRpc

    [ServerRpc(RequireOwnership = false)]
    private void FlashEffectServerRpc(ulong clientId, float destance)
    {
        FlashEffectClientRpc(clientId, destance);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShakeCaneraServerRpc(ulong clientId, float amplitude, float freauency, float time)
    {
        ShakeCaneraClientRpc(clientId, amplitude, freauency, time);
    }

    #endregion

    #region ClientRpc

    [ClientRpc]
    private void FlashEffectClientRpc(ulong clientId, float destance)
    {
        if (clientId != NetworkManager.LocalClientId) return;

        DOTween.Kill(transform);
        flashImage.gameObject.SetActive(true);
        DOTween.Sequence()
            .Append(flashImage.DOFade(1, 0.1f))
            .Append(flashImage.DOFade(1, (destance / flashProportion) * (flashProportion - 1)))
            .Append(flashImage.DOFade(0, destance / flashProportion).SetEase(Ease.InQuart))
            .OnComplete(() => { flashImage.gameObject.SetActive(false); });
    }

    [ClientRpc()]
    private void ShakeCaneraClientRpc(ulong clientId, float amplitude, float freauency, float time)
    {
        if (clientId != NetworkManager.LocalClientId) return;

        StopAllCoroutines();
        StartCoroutine(ShakeEffect(amplitude, freauency, time));
    }

    #endregion

    private IEnumerator ShakeEffect(float amplitude, float freauency, float time)
    {
        vcamNoise.m_AmplitudeGain = amplitude;
        vcamNoise.m_FrequencyGain = freauency;

        yield return new WaitForSecondsRealtime(time);

        vcamNoise.m_AmplitudeGain = 0;
        vcamNoise.m_FrequencyGain = 0;
    }
}
