using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPC))]
public abstract class AIGlobalBehavior : MonoBehaviour
{
    [HideInInspector] public NPC myNPC;
    protected AIPath aiAgent;
    protected AIDestinationSetter destinationSetter;

    public virtual void Awake()
    {
        myNPC = GetComponent<NPC>();
        aiAgent = GetComponentInParent<AIPath>();
        destinationSetter = GetComponentInParent<AIDestinationSetter>();
    }

    public abstract void DoUpdate();

    public virtual void Stun() { }

    public virtual void ReactToTarget() { }
    public virtual float GetVelocity() { return 0; }
    public virtual float ComputeSpeed() { return 0; }
}