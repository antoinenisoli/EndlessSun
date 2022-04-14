using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static PlayerController2D Player;

    private void Awake()
    {
        Singleton();
        Player = FindObjectOfType<PlayerController2D>();
    }

    void Singleton()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void FreezeFrame(float delay, float startScale = 0.1f)
    {
        StartCoroutine(IFreezeFrame(delay, startScale));
    }

    IEnumerator IFreezeFrame(float delay, float startScale = 0.1f)
    {
        float timer = 0;
        Time.timeScale = startScale;

        while (timer < delay)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1, timer / delay);
        }

        Time.timeScale = 1f;
    }
}
