using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CustomAI.BehaviorTree;

[CustomEditor(typeof(RegularBT))]
public class NPCBehaviorDebugger : Editor
{
    Actor actor;

#if UNITY_EDITOR

    private void OnSceneGUI()
    {
        RegularBT tree = (RegularBT)target;
        if (!actor)
            actor = tree.getActor();

        tree.chaseRange.Draw(actor);
        tree.aggroRange.Draw(actor);
        tree.attack.attackRange.Draw(actor);
        tree.sightRange.Draw(actor);
    }

#endif
}
