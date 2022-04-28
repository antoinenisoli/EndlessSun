using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public abstract class BehaviorTree : AIGlobalBehavior
    {
        AINode root;

        public void Start()
        {
            root = MakeTree();
        }

        public override void DoUpdate()
        {
            if (root != null)
                root.Evaluate();
        }

        public virtual AINode MakeTree() => null;
    }
}