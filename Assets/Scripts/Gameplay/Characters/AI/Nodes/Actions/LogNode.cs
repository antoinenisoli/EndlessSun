using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    public class LogNode : AIActionNode
    {
        string text;

        public LogNode(string text)
        {
            this.text = text;
        }

        public override void OnStart()
        {
            base.OnStart();
            Debug.Log(text);
        }

        public override void Execute()
        {
            Debug.Log(text);
        }

        public override bool Step()
        {
            return true;
        }
    }
}