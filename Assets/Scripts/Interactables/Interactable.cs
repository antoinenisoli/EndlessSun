using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public void ProposeToInteract()
    {
        UIManager.Instance.ShowPickUp(this);
    }

    public abstract void Interact();
}
