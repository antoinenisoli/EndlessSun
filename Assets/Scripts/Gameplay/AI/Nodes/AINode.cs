using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeState
{
    Running,
    Success,
    Failure,
}

namespace CustomAI.BehaviorTree
{
    public abstract class AINode
    {
        public NodeState nodeState;
        public AINode parent;
        [HideInInspector] public bool started = false;
        public List<AINode> childrens = new List<AINode>();
        public virtual string name => GetType().Name;
        Dictionary<string, object> dataContext = new Dictionary<string, object>();

        public AINode(List<AINode> childrens)
        {
            foreach (var child in childrens)
                Attach(child);
        }

        public AINode()
        {
            parent = null;
        }

        public void Attach(params AINode[] nodes)
        {
            foreach (var node in nodes)
            {
                node.parent = this;
                childrens.Add(node);
            }
        }

        public void SetData(string key, object value)
        {
            dataContext.Add(key, value);
        }

        public object GetData(string key)
        {
            object value = null;
            if (dataContext.TryGetValue(key, out value))
                return value;

            AINode node = parent;
            while (node != null)
            {
                value = node.GetData(key);
                if (value != null)
                    return value;

                node = node.parent;
            }

            return null;
        }

        public bool ClearData(string key)
        {
            object value = null;
            if (dataContext.ContainsKey(key))
            {
                dataContext.Remove(key);
                return true;
            }

            AINode node = parent;
            while (node != null)
            {
                bool cleared = node.ClearData(key);
                if (cleared)
                    return true;

                node = node.parent;
            }

            

            return false;
        }

        public virtual void OnStart() 
        { 
            //Debug.Log(name + " started");
            started = true;
        }

        public virtual void OnStop()
        {
            started = false;
        }

        public void StartCheck()
        {
            if (!started)
                OnStart();
        }

        public void StopCheck()
        {
            if (nodeState != NodeState.Running)
                OnStop();
        }

        public abstract NodeState Evaluate();
    }
}