using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Netcode;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance;

    [SerializeField] private Volume volume;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        Instance = this;
    }

    public void BloomIntensity(float value)
    {
        if (volume.profile.TryGet(out Bloom bloom))
        {
            bloom.intensity.value = Mathf.Lerp(bloom.intensity.value, value, 5f * Time.deltaTime);
        }
    }

    public void FilmGrainIntensity(float value)
    {
        if (volume.profile.TryGet(out FilmGrain filmGrain))
        {
            filmGrain.intensity.value = Mathf.Lerp(filmGrain.intensity.value, value, 5f * Time.deltaTime);
        }
    }

    public void MotionBlurIntensity(float value)
    {
        if (volume.profile.TryGet(out MotionBlur motionBlur))
        {
            motionBlur.intensity.value = Mathf.Lerp(motionBlur.intensity.value, value, 5f * Time.deltaTime);
        }
    }

    public void VignetteIntensity(float value)
    {
        if (volume.profile.TryGet(out Vignette vignette))
        {
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, value, 5f * Time.deltaTime);
        }
    }

    public void VignetteColor(Color color)
    {
        if (volume.profile.TryGet(out Vignette vignette))
        {
            vignette.color.value = color;
        }
    }
}
