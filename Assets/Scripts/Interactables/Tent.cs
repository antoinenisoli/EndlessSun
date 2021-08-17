using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tent : Interactable
{
    public override void Interact()
    {
        PlayerSurvival.Energy.Init();
        EventManager.Instance.onPlayerSleep.Invoke();
    }

    public override string ToString()
    {
        return "Sleep";
    }
}
