using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI
{
    [System.Serializable]
    public class PatrolData
    {
        public NPC myNPC;
        public float minNextWaypointDistance = 3f;
        public float chaseMinDistance = 2f, attackMinDistance = 2f;
        public float aggroDistance = 20f, visionDistance = 50f;
        public Vector2 randomDelayBounds;
        public Vector2 randomPatrolRange;
        public ShowRectangleGizmo patrolGizmo;

        public bool DetectTarget(out Entity target)
        {
            target = null;
            Collider2D[] colls = Physics2D.OverlapCircleAll(myNPC.transform.position, aggroDistance);
            if (colls.Length > 0)
            {
                foreach (var item in colls)
                {
                    Entity entity = item.GetComponent<Entity>();
                    if (entity && !myNPC.SameTeam(entity))
                    {
                        float distance = Vector2.Distance(entity.transform.position, myNPC.transform.position);
                        if (distance < aggroDistance && !entity.Health.isDead)
                        {
                            target = entity;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        Vector2 RandomPatrolPosition()
        {
            Vector2 vector = GameDevHelper.RandomVector(randomPatrolRange, myNPC.startPosition);
            return vector;
        }

        public void Move(Vector3 targetPos)
        {
            float distance = Vector2.Distance(targetPos, myNPC.transform.position);
            float stopDistance = myNPC.State == AIState.Patrolling ? myNPC.aiAgent.endReachedDistance : chaseMinDistance;

            if (distance > stopDistance)
            {
                myNPC.aiAgent.enabled = true;
                myNPC.aiAgent.isStopped = false;
                myNPC.destinationPoint.position = targetPos;
            }
            else
                myNPC.Stop();
        }

        public float RandomDelay()
        {
            return GameDevHelper.RandomInRange(randomDelayBounds);
        }

        public Vector2 NewDestination()
        {
            myNPC.Stop();
            Vector2 randomPos = RandomPatrolPosition();
            if (GridManager.Instance)
            {
                if (GridManager.Instance.SamplePosition(randomPos, 2f, out Vector2 sampledPos))
                    return sampledPos;
            }

            return randomPos;
        }

        public void Gizmos()
        {
            patrolGizmo.gameObject.SetActive(true);

            if (patrolGizmo)
                patrolGizmo.SetSize(randomPatrolRange * 2);
        }
    }
}