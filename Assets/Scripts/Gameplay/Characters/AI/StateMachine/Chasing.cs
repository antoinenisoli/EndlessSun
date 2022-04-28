using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomAI
{
    [System.Serializable]
    class Chasing : SubBehavior
    {
        public override AIState State => AIState.Chasing;
        Vector2 targetPos => behavior.actor.Target.transform.position;

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
            if (!behavior.KeepTargetInSight() || behavior.actor.Target.Health.isDead || myNPC.SameTeam(myNPC.Target))
            {
                myNPC.SetTarget(null);
                behavior.SetBehaviour(new Wait(behavior, 2, AIState.Patrolling));
                return;
            }

            if (myNPC.CheckDistance(5))
                attackBehavior.Update();
            else
                behavior.Move(targetPos);
        }
    }
}