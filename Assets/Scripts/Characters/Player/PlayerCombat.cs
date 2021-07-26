using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerCombat
{
    PlayerController2D player => GameManager.Player;
    public static PlayerStat Mana;
    public static StaminaStat Stamina;

    [SerializeField] StaminaStat stamina;
    public PlayerStat[] stats = new PlayerStat[3];
    Dictionary<PlayerStatName, PlayerStat> dico = new Dictionary<PlayerStatName, PlayerStat>();

    [Header("Attack")]
    [SerializeField] float staminaCost = 15f;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] int attackAnimCount = 3;
    [Range(0, 1)] public float attackRadius = 0.5f, attackRange = 0.5f;
    [SerializeField] float pushForce = 5f;

    [Header("Bow")]
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] float arrowForce = 15f;
    public Vector2 storedVelocity;

    public void Init()
    {
        foreach (var item in stats)
        {
            dico.Add(item.thisStat, item);
            item.Init(); 
        }

        Mana = dico[PlayerStatName.Mana];
        Stamina = stamina;
        Stamina.Init();
        dico.Add(Stamina.thisStat, Stamina);
    }

    public PlayerStat GetCombatStat(PlayerStatName statName)
    {
        if (dico.TryGetValue(statName, out PlayerStat stat))
            return stat;

        return null;
    }

    public void Attack()
    {
        Stamina.StaminaCost(staminaCost);
        player.SetState(PlayerState.Idle);
        RaycastHit2D[] colls = Physics2D.CircleCastAll(player.transform.position, attackRadius, player.spr.transform.right, attackRange, enemyLayer);
        player.idleSword = true;
        if (colls.Length > 0)
            GameManager.Instance.CameraShake(0.2f);
        else
            return;

        foreach (var item in colls)
        {
            Enemy enemy = item.transform.GetComponent<Enemy>();
            if (enemy)
                enemy.Hit(-item.normal * pushForce);
        }
    }

    public void FireArrow()
    {
        Stamina.StaminaCost(staminaCost);
        player.SetState(PlayerState.Idle);
        GameObject arrow = Object.Instantiate(arrowPrefab, player.transform.position, arrowPrefab.transform.rotation);
        Rigidbody2D arrowRB = arrow.GetComponent<Rigidbody2D>();
        float rot_z = Mathf.Atan2(storedVelocity.y, storedVelocity.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
        arrowRB.AddForce(storedVelocity * arrowForce, ForceMode2D.Impulse);
    }

    public void Update()
    {
        float computeEnergy = PlayerSurvival.Energy.MaxValue - PlayerSurvival.Energy.CurrentValue;
        Stamina.MaxValue = Stamina.BaseMaxValue - computeEnergy;
        Stamina.Update();

        float computeHunger = PlayerSurvival.Hunger.MaxValue - PlayerSurvival.Hunger.CurrentValue;
        Mana.MaxValue = Mana.BaseMaxValue - computeHunger;
    }
}
