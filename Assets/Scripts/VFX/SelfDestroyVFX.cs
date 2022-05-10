using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroyVFX : MonoBehaviour
{
    [SerializeField] float delay = 3f;

    IEnumerator Start()
    {
        ParticleSystem fx = GetComponent<ParticleSystem>();
        if (fx)
        {
            fx.Play();
            yield return new WaitForSeconds(fx.main.duration + fx.main.startLifetimeMultiplier);
        }
        else
            yield return new WaitForSeconds(delay);

        Destroy(gameObject);
    }
}
