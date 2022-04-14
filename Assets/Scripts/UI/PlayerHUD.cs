using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : HUD
{
    [SerializeField] Slider thirstySlider;

    [Header("Health")]
    [SerializeField] SpriteColorAnimation colorAnimation;
    [SerializeField] int heartPulse = 5;
    [SerializeField] Slider healthSlider;

    [Header("Mana")]
    [SerializeField] Slider manaSlider;
    [SerializeField] Slider hungerSlider;

    [Header("Stamina")]
    [SerializeField] Slider staminaSlider;
    [SerializeField] Slider energySlider;

    private void Start()
    {
        if (!GameManager.Player)
            return;

        healthSlider.value = GameManager.Player.Health.Coeff();
        thirstySlider.value = PlayerSurvival.Instance.Thirsty.Coeff();

        manaSlider.maxValue = PlayerMagic.Mana.MaxValue;
        manaSlider.value = PlayerMagic.Mana.CurrentValue;

        staminaSlider.maxValue = PlayerCombat.Stamina.MaxValue;
        staminaSlider.value = PlayerCombat.Stamina.CurrentValue;

        hungerSlider.maxValue = PlayerSurvival.Instance.Hunger.MaxValue;
        hungerSlider.value = PlayerSurvival.Instance.Hunger.CurrentValue;

        energySlider.maxValue = PlayerSurvival.Instance.Energy.MaxValue;
        energySlider.value = PlayerSurvival.Instance.Energy.CurrentValue;

        UpdateUI();
    }

    void UpdateMana()
    {
        float compute = PlayerSurvival.Instance.Hunger.MaxValue - PlayerSurvival.Instance.Hunger.CurrentValue;
        if (float.IsNaN(compute) || float.IsInfinity(compute))
            return;

        hungerSlider.value = compute;
        if (PlayerMagic.Mana != null)
            manaSlider.value = PlayerMagic.Mana.CurrentValue;
    }

    void UpdateStamina()
    {
        float compute = PlayerSurvival.Instance.Energy.MaxValue - PlayerSurvival.Instance.Energy.CurrentValue;
        if (float.IsNaN(compute) || float.IsInfinity(compute))
            return;

        energySlider.value = compute;
        if (PlayerCombat.Stamina != null)
            staminaSlider.value = PlayerCombat.Stamina.CurrentValue;
    }

    void UpdateHealth()
    {
        float compute = GameManager.Player.Health.Coeff();
        if (float.IsNaN(compute) || float.IsInfinity(compute))
            return;

        //print(computeHealth);
        healthSlider.value = compute;
        thirstySlider.value = 1 - compute;
        colorAnimation.time = 0.3f + (heartPulse * compute);
    }

    public override void UpdateUI()
    {
        if (PlayerSurvival.Instance)
        {
            UpdateMana();
            UpdateStamina();
            UpdateHealth();
        }
    }
}
