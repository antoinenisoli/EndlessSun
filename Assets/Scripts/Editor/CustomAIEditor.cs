using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class CustomAIEditor : Editor
{
#if UNITY_EDITOR

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
        string path = Application.dataPath + "/Scripts/Gameplay/Characters/AI" + $"{filePath}.cs";
        using (StreamWriter streamWriter = new StreamWriter(path))
            streamWriter.Write(CodeText());

        Debug.Log("Created node at " + path);
        AssetDatabase.Refresh();
        AssetDatabase.LoadMainAssetAtPath(path);
    }

#endif
}
