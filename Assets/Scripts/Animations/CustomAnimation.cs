using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomAnimation
{
    enum AnimExecution
    {
        Once,
        Loop,
    }

    public string name = "Idle";
    public float frameRate = 0.1f, speed = 1;
    public bool destroyOnEnd;
    [SerializeField] AnimExecution execution;
    CustomAnimator myAnimator;
    float timer;
    int spriteIndex;
    bool active = true;
    [HideInInspector] public Sprite mainSprite;

    public Sprite[] sprites;

    public void Init(CustomAnimator animator, bool active = true)
    {
        myAnimator = animator;
        mainSprite = sprites[0];
        this.active = active;
    }

    public void Update()
    {
        if (!active)
            return;

        timer += Time.deltaTime * speed;
        if (timer > frameRate)
        {
            timer = 0;
            spriteIndex++;
            if (spriteIndex % sprites.Length == 0)
            {
                End();
                if (execution == AnimExecution.Once)
                    return;
            }

            spriteIndex %= sprites.Length;
            mainSprite = sprites[spriteIndex];
        }
    }

    public void PlayAnim()
    {
        active = true;
        Reset();
    }

    void End()
    {
        if (destroyOnEnd)
            Object.Destroy(myAnimator.gameObject);

        switch (execution)
        {
            case AnimExecution.Once:
                active = false;
                break;
            case AnimExecution.Loop:
                break;
        }
    }

    public void Reset()
    {
        timer = 0;
        spriteIndex = 0;
        mainSprite = sprites[spriteIndex];
    }
}
