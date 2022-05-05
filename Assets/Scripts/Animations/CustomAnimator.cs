using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomAnimator : MonoBehaviour
{
    [SerializeField] string startAnim = "Idle";
    [SerializeField] bool playOnStart = true;
    [SerializeField] protected SpriteRenderer animRenderer;
    [SerializeField] protected List<CustomAnimation> animations = new List<CustomAnimation>();
    Dictionary<string, CustomAnimation> customAnimations = new Dictionary<string, CustomAnimation>();
    CustomAnimation currentAnim = null;

    private void Awake()
    {
        foreach (var item in animations)
        {
            item.Init(this);
            customAnimations.Add(item.name, item);
        }

        PlayAnimation(startAnim, playOnStart); 
    }

    public CustomAnimation GetAnimation(string stateName)
    {
        if (customAnimations.TryGetValue(stateName, out CustomAnimation anim))
            return anim;
        else
            Debug.LogError("Didn't found this animation state : " + stateName);

        return null;
    }

    public void RemoveAnimation(CustomAnimation anim)
    {
        if (anim != null && customAnimations.ContainsValue(anim))
        {
            animations.Remove(anim);
            customAnimations.Remove(anim.name);
            if (anim == currentAnim)
                currentAnim = null;
        }
    }

    public void PlayAnimation(string stateName, bool play = true)
    {
        currentAnim = GetAnimation(stateName);
        if (play && currentAnim != null)
            currentAnim.PlayAnim();
    }

    public void Flip()
    {
        transform.Rotate(Vector3.up * 180);
    }

    private void Update()
    {
        if (currentAnim != null)
        {
            currentAnim.Update();
            animRenderer.sprite = currentAnim.mainSprite;
        }
    }
}
