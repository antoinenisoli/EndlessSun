using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using CustomAI.BehaviorTree;

public class CustomAIEditor : Editor
{
#if UNITY_EDITOR

    static string assetPath = "/Scripts/Gameplay/AI/Nodes/";

    public static string CodeText()
    {
        StringBuilder builder = new StringBuilder();
        string[] lines = File.ReadAllLines(@"Assets/Scripts/Editor/NodeCodeTemplate.txt");
        foreach (var item in lines)
            builder.AppendLine(item);

        return builder.ToString();
    }

    [MenuItem("Tools/CustomAI/New node", false, -1)]
    public static void NewNode()
    {
        string filePath = "NewAINode";
        string path = Application.dataPath + $"/Scripts/Gameplay/AI/Nodes/{filePath}.cs";
        using (StreamWriter streamWriter = new StreamWriter(path))
            streamWriter.Write(CodeText());

        Debug.Log("Created node at " + path);
        Ping(filePath);
    }

    public static void Ping(string filePath)
    {
        string path = $"Assets/Scripts/Gameplay/AI/Nodes/{filePath}.cs";
        AssetDatabase.Refresh();
        Object other = AssetDatabase.LoadMainAssetAtPath(path);
        EditorGUIUtility.PingObject(other);
    }

#endif
}
