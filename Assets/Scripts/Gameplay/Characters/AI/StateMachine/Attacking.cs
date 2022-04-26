using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
class Attacking : SubBehavior
{
    public override AIState State => AIState.Attacking;
    float timer;
    Vector2 randomPosition;
    float randomTimer;

    public Attacking(AIStateMachineBehavior behavior) : base(behavior)
    {
        myNPC.Stop();
        randomTimer = UnityEngine.Random.Range(1f, 3f);
        randomPosition = myNPC.Target.transform.position;
    }

    public override void Update()
    {
        base.Update();

        if (myNPC.Target.Health.isDead)
        {
            myNPC.SetTarget(null);
            behavior.SetBehaviour(new Wait(behavior, 2, AIState.Patrolling));
            return;
        }
        else if (!behavior.NearToTarget())
            behavior.SetBehaviour(new Chasing(behavior));

        if (timer >= myNPC.attackRate)
        {
            timer = 0;
            myNPC.LaunchAttack();
        }
        else
            timer += Time.deltaTime;

        float distance = Vector2.Distance(myNPC.transform.position, myNPC.Target.transform.position);
        Vector2 diff = myNPC.transform.position - myNPC.Target.transform.position;
        diff.Normalize();
        if (distance < 1f)
        {
            Debug.Log(diff);
            behavior.Move((Vector2)myNPC.transform.position + (diff * 6f));
            randomTimer = UnityEngine.Random.Range(1f, 3f);
            return;
        }

        randomTimer -= Time.deltaTime;
        if (randomTimer <= 0)
            NewPos();
    }

    void NewPos()
    {
        randomTimer = UnityEngine.Random.Range(1f, 3f);
        float radius = 2f;
        Vector2 random = UnityEngine.Random.insideUnitCircle * radius;
        randomPosition = (Vector2)myNPC.Target.transform.position + random;
        float distance = Vector2.Distance(randomPosition, myNPC.Target.transform.position);

        while (distance < 2f)
        {
            random = UnityEngine.Random.insideUnitCircle * radius;
            randomPosition = (Vector2)myNPC.Target.transform.position + random;
            distance = Vector2.Distance(randomPosition, myNPC.Target.transform.position);
        }

        Debug.Log("new pos " + randomPosition);
        behavior.Move(randomPosition);
    }
}
