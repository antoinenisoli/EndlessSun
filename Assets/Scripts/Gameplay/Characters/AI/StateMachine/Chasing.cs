using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
class Chasing : StateMachineBehavior
{
    public override AIState State => AIState.Chasing;

    public Chasing(Enemy myEnemy) : base(myEnemy)
    {
        
    }

    public override void Gizmos()
    {
        base.Gizmos();
        myEnemy.visionGizmo.gameObject.SetActive(true);
        if (myEnemy.visionGizmo)
            myEnemy.visionGizmo.SetSize(myEnemy.visionDistance);
    }

    public override void Update()
    {
        base.Update();
        if (myEnemy.NearToTarget())
            myEnemy.SetBehaviour(new Attacking(myEnemy));
        else if (!myEnemy.KeepTargetInSight() || myEnemy.Target.Health.isDead)
        {
            myEnemy.SetTarget(null);
            myEnemy.SetBehaviour(new Wait(myEnemy, 2, AIState.Patrolling));
        }
        else
            myEnemy.Move(myEnemy.Target.transform.position);
    }
}
