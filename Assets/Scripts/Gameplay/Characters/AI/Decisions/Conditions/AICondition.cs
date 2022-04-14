using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AICondition
{
    protected PlayerController2D player;
    public AIAction nodeYes, nodeNo;

    public abstract void Check();
}
