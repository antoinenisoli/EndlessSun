using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class PlayerCombat : PlayerMod
{
    public static StaminaStat Stamina;

    [Header("Attack")]
    [SerializeField] float attackStaminaCost = 15f;
    [SerializeField] int attackAnimCount = 3;
    [Range(0, 1)] public float attackRadius = 0.5f, attackRange = 0.5f;
    [SerializeField] float pushForce = 5f;

    [Header("Bow")]
    [SerializeField] float bowStaminaCost = 15f;
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] float arrowForce = 15f;
    public Vector2 storedVelocity;

    public override void Init()
    {
        base.Init();
        PlayerController2D.Combat = this;
        Stamina = playerProfile.Stamina;
        Stamina.Init(player);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(player.transform.position, player.spriteRenderer.transform.right * attackRange);
        Gizmos.DrawWireSphere(player.transform.position + player.spriteRenderer.transform.right * attackRange, attackRadius);
        Color clone = Color.red;
        clone.a = 0.25f;
        Gizmos.color = clone;
        Gizmos.DrawSphere(player.transform.position + player.spriteRenderer.transform.right * attackRange, attackRadius);
    }

    public bool EnoughStamina()
    {
        return Stamina.CurrentValue > 0;
    }

    public void Attack()
    {
        Stamina.StaminaCost(attackStaminaCost);
        player.SetPlayerState(PlayerState.InFight);
        RaycastHit2D[] colls = Physics2D.CircleCastAll(player.transform.position, attackRadius, player.spriteRenderer.transform.right, attackRange, player.targetLayer);
        if (colls.Length > 0)
            CameraManager.Instance.CameraShake(0.15f, 15);
        else
            return;

        foreach (var item in colls)
        {
            Entity entity = item.transform.GetComponentInChildren<Entity>();
            if (entity == player)
                return;

            if (entity && !entity.Health.isDead && player.IsHostile(entity))
                entity.TakeDamages(player.ComputeDamages(), player, item.point);
        }
    }

    public void FireArrow()
    {
        Stamina.StaminaCost(bowStaminaCost);
        player.SetPlayerState(PlayerState.Idle);
        GameObject arrow = Instantiate(arrowPrefab, player.transform.position, arrowPrefab.transform.rotation);
        Rigidbody2D arrowRB = arrow.GetComponent<Rigidbody2D>();
        float rot_z = Mathf.Atan2(storedVelocity.y, storedVelocity.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
        arrowRB.AddForce(storedVelocity * arrowForce, ForceMode2D.Impulse);
    }

    public override void DoUpdate()
    {
        float computeEnergy = PlayerSurvival.Instance.Energy.MaxValue - PlayerSurvival.Instance.Energy.CurrentValue;
        Stamina.MaxValue = Stamina.BaseMaxValue - computeEnergy;
        Stamina.Update();
    }
}
