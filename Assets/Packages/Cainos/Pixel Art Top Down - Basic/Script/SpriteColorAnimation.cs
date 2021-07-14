using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//animate the sprite color base on the gradient and time
public class SpriteColorAnimation : MonoBehaviour
{
    public Gradient gradient;
    public float time;

    [SerializeField] Image image;
    [SerializeField] SpriteRenderer sr;
    float timer;

    private void Start()
    {
        timer = time * Random.value;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > time) timer = 0.0f;

        if (sr)
            sr.color = gradient.Evaluate(timer / time);
        if (image)
            image.color = gradient.Evaluate(timer / time);
    }
}
