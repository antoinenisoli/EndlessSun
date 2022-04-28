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
            Debug.Log("start new tree !");
        }

        public override void DoUpdate()
        {
            if (root != null)
            {
                if (root.Evaluate() != NodeState.Running)
                    Start();
            }
        }

        public virtual AINode MakeTree() => null;
    }
}