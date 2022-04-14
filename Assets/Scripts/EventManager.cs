using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
    public UnityEvent onLevelUp = new UnityEvent();
    public UnityEvent onPlayerSleep = new UnityEvent();
    public UnityEvent onPlayerAwake = new UnityEvent();
    public UnityEvent onMapCreated = new UnityEvent();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}
