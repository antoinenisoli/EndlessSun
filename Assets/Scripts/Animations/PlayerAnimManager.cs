using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimManager : MonoBehaviour
{
    public void SwordAttack()
    {
        PlayerController2D.Combat.Attack();
    }

    public void BowAttack()
    {
        PlayerController2D.Combat.FireArrow();
    }
}
