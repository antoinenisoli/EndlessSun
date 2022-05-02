using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI
{
    [System.Serializable]
    public class PatrolData
    {
        public NPC myNPC;
        public Vector2 randomDelayBounds;
        public Vector2 randomPatrolRange;
        public ShowRectangleGizmo patrolGizmo;

        Vector2 RandomPatrolPosition()
        {
            Vector2 vector = GameDevHelper.RandomVector(randomPatrolRange, myNPC.startPosition);
            return vector;
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