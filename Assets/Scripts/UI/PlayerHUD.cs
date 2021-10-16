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
        healthSlider.maxValue = GameManager.Player.health.MaxValue;
        healthSlider.value = GameManager.Player.health.CurrentValue;

        manaSlider.maxValue = PlayerCombat.Mana.MaxValue;
        manaSlider.value = PlayerCombat.Mana.CurrentValue;

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
        manaSlider.value = PlayerCombat.Mana.CurrentValue;
    }

    void UpdateStamina()
    {
        float stamina = PlayerSurvival.Instance.Energy.MaxValue - PlayerSurvival.Instance.Energy.CurrentValue;
        energySlider.value = stamina;
        staminaSlider.value = PlayerCombat.Stamina.CurrentValue;
    }

    void UpdateThirst()
    {
        float computeThirst = PlayerSurvival.Instance.Thirsty.CurrentValue;
        thirstySlider.value = computeThirst;
    }

    public override void UpdateUI()
    {
        float computeHealth = GameManager.Player.health.CurrentValue / GameManager.Player.health.MaxValue; 
        healthSlider.DOValue(GameManager.Player.health.CurrentValue, 0.3f);
        colorAnimation.time = 0.3f + (heartPulse * computeHealth);

        UpdateMana();
        UpdateStamina();
        UpdateThirst();
    }
}
