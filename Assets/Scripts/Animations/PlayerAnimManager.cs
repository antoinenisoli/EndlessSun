using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimManager : MonoBehaviour
{
    public void SwordAttack()
    {
        GameManager.Player.Attack();
    }

    public void BowAttack()
    {
        GameManager.Player.FireArrow();
    }
}
