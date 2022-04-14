using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIBehavior : MonoBehaviour
{
    public Entity myEntity;

    public abstract void DoUpdate();

    public virtual void Stun() { }

    public virtual void React() { }
}
