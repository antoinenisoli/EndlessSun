using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
class Chasing : SubBehavior
{
    public override AIState State => AIState.Chasing;
    Vector2 targetPos => behavior.myNPC.Target.transform.position;

    Attacking attackBehavior;

    public Chasing(AIStateMachineBehavior behavior) : base(behavior)
    {
        attackBehavior = new Attacking(behavior);
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
        if (!behavior.KeepTargetInSight() || behavior.myNPC.Target.Health.isDead || myNPC.SameTeam(myNPC.Target))
        {
            myNPC.SetTarget(null);
            behavior.SetBehaviour(new Wait(behavior, 2, AIState.Patrolling));
            return;
        }

        if (behavior.NearToTarget())
            attackBehavior.Update();
        else
            behavior.Move(targetPos);
    }

    Vector2 Test()
    {
        Vector2 direction = (Vector2)behavior.myNPC.transform.position - targetPos;
        float cross = Vector2.Dot(direction.normalized, targetPos.normalized);

        Vector2 randomOffset = Random.insideUnitCircle * 3f;
        return targetPos + randomOffset;
    }
}
