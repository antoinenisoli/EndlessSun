using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CustomAI.BehaviorTree;

//[CustomPropertyDrawer(typeof(RelationManager))]
public class RelationManagerEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        for (int i = 0; i < 5; i++)
        {
            EditorGUI.BeginProperty(position, label, property);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var effectRect = new Rect(position.x, position.y - i * 10, position.width, EditorGUIUtility.singleLineHeight);

            var teamProperty = property.FindPropertyRelative("myTeam");
            teamProperty.intValue = EditorGUI.Popup(effectRect, "test!!!", teamProperty.intValue, teamProperty.enumNames);
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}
