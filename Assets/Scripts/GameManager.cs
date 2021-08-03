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
}
