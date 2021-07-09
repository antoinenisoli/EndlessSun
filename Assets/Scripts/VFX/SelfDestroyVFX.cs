using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroyVFX : MonoBehaviour
{
    ParticleSystem fx => GetComponent<ParticleSystem>();

    IEnumerator Start()
    {
        fx.Play();
        yield return new WaitForSeconds(fx.main.duration + fx.main.startLifetimeMultiplier);
        Destroy(gameObject);
    }
}
