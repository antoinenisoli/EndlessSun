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
        public DistanceCheck attackRange = new DistanceCheck(2f, Color.white);

        public AttackNode GenerateNode()
        {
            switch (pattern)
            {
                case FightMovePattern.Straight:
                    return new AttackNode(myNPC, attackRange.range * 0.95f);
                case FightMovePattern.Circle:
                    return new AttackInCircleNode(myNPC, attackRange.range / 2);
                case FightMovePattern.Charge:
                    return new AttackNode(myNPC, attackRange.range * 0.95f);
            }

            return null;
        }
    }
}