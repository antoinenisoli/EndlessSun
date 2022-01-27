using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField] float unZoomValue = 15f;
    float baseZoomValue;
    float currentZoomValue;
    [HideInInspector] public CinemachineVirtualCamera cinemachineCam;

    private void Awake()
    {
        Singleton();
        cinemachineCam = FindObjectOfType<CinemachineVirtualCamera>();
        baseZoomValue = cinemachineCam.m_Lens.OrthographicSize;
        currentZoomValue = baseZoomValue;
    }

    void Singleton()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void UnZoom()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(DOTween.To(() => currentZoomValue, x => currentZoomValue = x, unZoomValue, 1));
        seq.SetLoops(2, LoopType.Yoyo);
    }

    public void CameraShake(float duration, float strength = 1, bool fadeOut = false)
    {
        StopAllCoroutines();
        CinemachineBasicMultiChannelPerlin noise = cinemachineCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = strength;
        noise.m_FrequencyGain = 1;
        StartCoroutine(Shaking(duration, fadeOut));
    }

    IEnumerator Shaking(float duration, bool fadeOut)
    {
        float timer = duration;
        CinemachineBasicMultiChannelPerlin noise = cinemachineCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
            if (fadeOut)
                noise.m_AmplitudeGain = Mathf.Lerp(noise.m_AmplitudeGain, 0, 1 - (timer / duration));
        }

        noise.m_AmplitudeGain = 0;
    }

    private void Update()
    {
        cinemachineCam.m_Lens.OrthographicSize = Mathf.Lerp(cinemachineCam.m_Lens.OrthographicSize, currentZoomValue, 5 * Time.deltaTime);
    }
}
