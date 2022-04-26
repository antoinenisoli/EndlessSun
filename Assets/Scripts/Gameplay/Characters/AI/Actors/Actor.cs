using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Actor : Entity
{
    [SerializeField] protected AIGlobalBehavior myBehavior;

    public override float ComputeSpeed()
    {
        return myBehavior.ComputeSpeed();
    }
}
