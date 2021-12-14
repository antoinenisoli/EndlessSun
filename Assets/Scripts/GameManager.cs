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

    public static Color RandomColor()
    {
        Color randomColor = new Color(
          Random.Range(0f, 1f),
          Random.Range(0f, 1f),
          Random.Range(0f, 1f)
            );

        return randomColor;
    }

    public static T RandomEnum<T>()
    {
        System.Array array = System.Enum.GetValues(typeof(IslandBiome));
        T randomBiome = (T)array.GetValue(Random.Range(0, array.Length));
        return randomBiome;
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
