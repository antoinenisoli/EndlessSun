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
}
