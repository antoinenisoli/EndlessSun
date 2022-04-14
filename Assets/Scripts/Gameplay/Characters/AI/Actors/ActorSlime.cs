using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorSlime : Actor
{
    [Header("Slime")]
    [SerializeField] AIBrain Brain;

    private void Start()
    {
        SetState(DefaultState);
    }

    void OnDrawGizmosSelected()
    {
       
    }

    public override void Update()
    {
        base.Update();
        Brain.Update();
    }
}
