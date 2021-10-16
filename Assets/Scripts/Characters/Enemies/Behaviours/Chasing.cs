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

    public Chasing(CharacterController player, Enemy myEnemy) : base(player, myEnemy)
    {
        
    }

    public override void Update()
    {
        base.Update();
        /*if (myEnemy.NearPlayer())
            myEnemy.SetBehaviour(new Attacking(player, myEnemy));
        else
            myEnemy.Move(player.transform.position);*/
    }
}
