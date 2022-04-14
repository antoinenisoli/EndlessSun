using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
class Chasing : SubBehavior
{
    public override AIState State => AIState.Chasing;

    public Chasing(AIStateMachineBehavior behavior) : base(behavior)
    {
        
    }

    public override void Gizmos()
    {
        base.Gizmos();
        behavior.visionGizmo.gameObject.SetActive(true);
        if (behavior.visionGizmo)
            behavior.visionGizmo.SetSize(behavior.visionDistance);
    }

    public override void Update()
    {
        base.Update();
        if (behavior.NearToTarget())
            behavior.SetBehaviour(new Attacking(behavior));
        else if (!behavior.KeepTargetInSight() || behavior.myNPC.Target.Health.isDead)
        {
            myNPC.SetTarget(null);
            behavior.SetBehaviour(new Wait(behavior, 2, AIState.Patrolling));
        }
        else
            behavior.Move(behavior.myNPC.Target.transform.position);
    }
}
