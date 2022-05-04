using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public enum FightMovePattern
    {
        Straight,
        Circle,
        Charge,
    }

    [System.Serializable]
    public class AttackData
    {
        public NPC myNPC;
        public FightMovePattern pattern;
        [SerializeField] Transform attackPoint;
        public float attackRadius = 1.5f;
        public float attackRate = 1f, stayAwaySpeed = 1f, movingAroundSpeed = 1f, stepBackDelay = 0.15f;
        public DistanceCheck attackRange = new DistanceCheck(2f, Color.white);

        public void Attack()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRadius);
            foreach (var item in colliders)
            {
                Entity entity = item.GetComponentInParent<Entity>();
                if (entity && !entity.Health.isDead && !myNPC.IsFriendly(entity))
                    entity.TakeDamages(myNPC.ComputeDamages(), myNPC);
            }
        }

        public AttackNode GenerateNode()
        {
            switch (pattern)
            {
                case FightMovePattern.Straight:
                    return new AttackNode(myNPC, attackRange.range * 0.95f, stepBackDelay, stayAwaySpeed);
                case FightMovePattern.Circle:
                    AttackInCircleNode inCircle = new AttackInCircleNode(myNPC, attackRange.range/2, stepBackDelay, stayAwaySpeed);
                    inCircle.SetMovingBehavior(false, movingAroundSpeed, new Vector2(2f, 4f));
                    return inCircle;
                case FightMovePattern.Charge:
                    return null;
                default:
                    return null; 
            }
        }
    }
}