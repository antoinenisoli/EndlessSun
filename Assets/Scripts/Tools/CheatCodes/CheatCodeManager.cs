using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatCodeManager : MonoBehaviour
{
    void EditorCodes()
    {
        if (Input.GetKeyDown(KeyCode.R))
            GameManager.Instance.Reload();

        if (Input.GetKeyDown(KeyCode.H))
            GameManager.Player.TakeDamages(25);

        if (Input.GetKeyDown(KeyCode.J))
            GameManager.Player.Heal(25);
    }

    void BuildCodes()
    {
        
    }

    private void Update()
    {
#if UNITY_EDITOR
        EditorCodes();
#endif
        BuildCodes();
    }
}
