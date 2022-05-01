using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CustomAI.BehaviorTree
{
    [System.Serializable]
    public struct DistanceCheck
    {
        public float range;
        public Color color;

        public DistanceCheck(float range, Color color)
        {
            this.range = range;
            this.color = color;
        }

#if UNITY_EDITOR
        public void Draw(Actor actor)
        {
            Handles.color = color;
            GUI.color = color;

            Vector3 pos = actor.transform.position;
            range =
                Handles.ScaleValueHandle(
                    range,
                    pos + actor.transform.right * range,
                    actor.transform.rotation,
                    5,
                    Handles.ConeHandleCap,
                    1
                );

            Handles.DrawWireDisc(pos, actor.transform.forward, range);
            Handles.Label(
              pos + (range + 0.5f) * Vector3.right,
              range.ToString("0.0")
            );
        }
#endif

    }
}