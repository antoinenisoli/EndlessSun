using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimManager : MonoBehaviour
{
    public void Attack()
    {
        GameManager.Player.Attack();
    }
}
