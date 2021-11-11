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
        healthSlider.maxValue = GameManager.Player.Health.MaxValue;
        healthSlider.value = GameManager.Player.Health.CurrentValue;

        manaSlider.maxValue = PlayerMagic.Mana.MaxValue;
        manaSlider.value = PlayerMagic.Mana.CurrentValue;

        staminaSlider.maxValue = PlayerCombat.Stamina.MaxValue;
        staminaSlider.value = PlayerCombat.Stamina.CurrentValue;

        hungerSlider.maxValue = PlayerSurvival.Instance.Hunger.MaxValue;
        hungerSlider.value = PlayerSurvival.Instance.Hunger.CurrentValue;

        thirstySlider.maxValue = PlayerSurvival.Instance.Thirsty.MaxValue;
        thirstySlider.value = PlayerSurvival.Instance.Thirsty.CurrentValue;

        energySlider.maxValue = PlayerSurvival.Instance.Energy.MaxValue;
        energySlider.value = PlayerSurvival.Instance.Energy.CurrentValue;
    }

    void UpdateMana()
    {
        float hunger = PlayerSurvival.Instance.Hunger.MaxValue - PlayerSurvival.Instance.Hunger.CurrentValue;
        hungerSlider.value = hunger;
        manaSlider.value = PlayerMagic.Mana.CurrentValue;
    }

    void UpdateStamina()
    {
        float stamina = PlayerSurvival.Instance.Energy.MaxValue - PlayerSurvival.Instance.Energy.CurrentValue;
        energySlider.value = stamina;
        staminaSlider.value = PlayerCombat.Stamina.CurrentValue;
    }

    void UpdateHealth()
    {
        float computeThirst = PlayerSurvival.Instance.Thirsty.Difference();
        thirstySlider.value = computeThirst;

        float computeHealth = GameManager.Player.Health.CurrentValue / GameManager.Player.Health.MaxValue;
        healthSlider.maxValue = GameManager.Player.Health.MaxValue;
        healthSlider.value = GameManager.Player.Health.CurrentValue;
        colorAnimation.time = 0.3f + (heartPulse * computeHealth);
    }

    public override void UpdateUI()
    {
        UpdateMana();
        UpdateStamina();
        UpdateHealth();
    }
}
