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
            Debug.Log("start new tree !");
            root = MakeTree();
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