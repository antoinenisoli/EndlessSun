using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeState
{
    Running,
    Success,
    Failure,
}

public abstract class AINode
{
    public NodeState nodeState;

    public abstract NodeState Evaluate();
}
