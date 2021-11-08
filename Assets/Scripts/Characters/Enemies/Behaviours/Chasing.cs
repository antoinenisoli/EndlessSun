using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
class Chasing : EnemyBehaviour
{
    public override EnemyState State => EnemyState.Chasing;

    public Chasing(Entity target, Enemy myEnemy) : base(target, myEnemy)
    {
        
    }

    public override void Update()
    {
        base.Update();
        if (myEnemy.NearToTarget(target.transform.position))
            myEnemy.SetBehaviour(new Attacking(target, myEnemy));
        else
            myEnemy.Move(target.transform.position);
    }
}
