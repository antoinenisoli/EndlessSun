using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    protected bool isOpen;
    public virtual void Open() { }
    public virtual void Close() { }

    public virtual void Switch()
    {
        isOpen = !isOpen;
        if (isOpen)
            Open();
        else
            Close();
    }
}
