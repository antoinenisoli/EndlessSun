using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerMod
{
    protected PlayerController2D player => GameManager.Player;

    public virtual void Init()
    {
        
    }

    public abstract void Update();
}
