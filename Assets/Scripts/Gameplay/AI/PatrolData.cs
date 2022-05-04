using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI
{
    [System.Serializable]
    public class PatrolData
    {
        public NPC myNPC;
        public float minDistance = 1f;
        public Vector2 randomDelayBounds;
        public Vector2 randomPatrolRange;
        public ShowRectangleGizmo patrolGizmo;

        public float RandomDelay()
        {
            return GameDevHelper.RandomInRange(randomDelayBounds);
        }

        public Vector2 NewDestination()
        {
            Vector2 randomPos = GameDevHelper.RandomVector(randomPatrolRange, myNPC.startPosition);
            int i = 100;

            while (i > 0)
            {
                randomPos = GameDevHelper.RandomVector(randomPatrolRange, myNPC.startPosition);
                float distance = Vector2.Distance(randomPos, myNPC.transform.position);
                if (distance > minDistance)
                    break;

                i--;
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