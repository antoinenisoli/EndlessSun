using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FeedbacksManager : MonoBehaviour
{
    public UnityEvent onPlayerDamage, onPlayerAttackHit;

    private void Start()
    {
        onPlayerDamage.AddListener(onPlayerAttackHit.Invoke);
    }

    public void Test()
    {
        Debug.Log("test");
    }

    [ContextMenu("test")]
    public void Test2()
    {
        onPlayerDamage.Invoke();
    }
}
