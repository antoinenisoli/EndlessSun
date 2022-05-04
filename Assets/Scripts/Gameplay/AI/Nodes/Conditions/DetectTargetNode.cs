using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class DetectTargetNode : AIConditionNode
    {
        Actor actor;
        float aggroDistance;

        public DetectTargetNode(Actor actor, float aggroDistance)
        {
            this.actor = actor;
            this.aggroDistance = aggroDistance;
        }

        public bool DetectTarget()
        {
            Collider2D[] colls = Physics2D.OverlapCircleAll(actor.transform.position, aggroDistance);
            if (colls.Length > 0)
            {
                foreach (var item in colls)
                {
                    Entity entity = item.GetComponent<Entity>();
                    if (entity && actor.IsHostile(entity))
                    {
                        float distance = Vector2.Distance(entity.transform.position, actor.transform.position);
                        if (distance < aggroDistance && !entity.Health.isDead)
                        {
                            Entity target = entity;
                            actor.NewTarget(target);
                            target.NewTarget(actor);
                            return true;
                        }
                    }
                }
            }

            actor.SetTarget(null);
            return false;
        }

        public override bool Check() => DetectTarget();
    }
}