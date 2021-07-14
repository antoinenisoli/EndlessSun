using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static PlayerController2D Player;
    CinemachineVirtualCamera mainCam;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Player = FindObjectOfType<PlayerController2D>();
        mainCam = FindObjectOfType<CinemachineVirtualCamera>();
    }

    public void CameraShake(float duration, float strength = 1, bool fadeOut = false)
    {
        StopAllCoroutines();
        CinemachineBasicMultiChannelPerlin noise = mainCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = strength;
        noise.m_FrequencyGain = 1;
        StartCoroutine(Shaking(duration, fadeOut));
    }

    IEnumerator Shaking(float duration, bool fadeOut)
    {
        float timer = duration;
        CinemachineBasicMultiChannelPerlin noise = mainCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
            if (fadeOut)
                noise.m_AmplitudeGain = Mathf.Lerp(noise.m_AmplitudeGain, 0, 1 - (timer / duration));
        }

        noise.m_AmplitudeGain = 0;
    }
}
