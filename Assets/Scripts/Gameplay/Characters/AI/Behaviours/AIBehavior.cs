using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIBehavior : MonoBehaviour
{
    protected Entity myEntity;

    public abstract void DoUpdate();
}
