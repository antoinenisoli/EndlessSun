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
        public float attackRate = 1f, movingAroundSpeed = 1f, stepBackDelay = 0.15f;
        public DistanceCheck attackRange = new DistanceCheck(2f, Color.white);

        public AttackNode GenerateNode()
        {
            switch (pattern)
            {
                case FightMovePattern.Straight:
                    return new AttackNode(myNPC, attackRange.range * 0.95f, stepBackDelay);
                case FightMovePattern.Circle:
                    AttackInCircleNode inCircle = new AttackInCircleNode(myNPC, attackRange.range/2, stepBackDelay, movingAroundSpeed);
                    inCircle.SetMovingBehavior(false, new Vector2(2f, 4f));
                    return inCircle;
                case FightMovePattern.Charge:
                    return null;
                default:
                    return null; 
            }
        }
    }
}