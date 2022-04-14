using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterMod : MonoBehaviour
{
    protected virtual Entity myCharacter => entity;
    [SerializeField] protected Entity entity;

    void Start()
    {
        Init();
    }

    public virtual void Init() 
    {
        myCharacter.AddMod(this);
    }

    public virtual void DoUpdate() { }

    public void Update() 
    {
        DoUpdate();
    }
}
