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

        hungerSlider.maxValue = PlayerSurvival.Hunger.MaxValue;
        hungerSlider.value = PlayerSurvival.Hunger.CurrentValue;

        thirstySlider.maxValue = PlayerSurvival.Thirsty.MaxValue;
        thirstySlider.value = PlayerSurvival.Thirsty.CurrentValue;

        energySlider.maxValue = PlayerSurvival.Energy.MaxValue;
        energySlider.value = PlayerSurvival.Energy.CurrentValue;
    }

    void UpdateMana()
    {
        float hunger = PlayerSurvival.Hunger.MaxValue - PlayerSurvival.Hunger.CurrentValue;
        hungerSlider.DOValue(hunger, 0.3f);
        if ((hungerSlider.maxValue - hungerSlider.value) < manaSlider.value)
            manaSlider.DOValue(PlayerCombat.Mana.CurrentValue - hunger, 0.3f);
        else
            manaSlider.DOValue(PlayerCombat.Mana.CurrentValue, 0.3f);
    }

    void UpdateStamina()
    {
        float stamina = PlayerSurvival.Energy.MaxValue - PlayerSurvival.Energy.CurrentValue;
        energySlider.DOComplete();
        staminaSlider.DOComplete();
        energySlider.DOValue(stamina, 0.3f);
        staminaSlider.DOValue(PlayerCombat.Stamina.CurrentValue, 0.3f);
    }

    void UpdateThirst()
    {
        float computeThirst = PlayerSurvival.Thirsty.CurrentValue;
        thirstySlider.DOValue(computeThirst, 0.5f);
    }

    public override void UpdateUI()
    {
        float computeHealth = GameManager.Player.health.CurrentValue;
        healthSlider.DOValue(computeHealth, 0.5f);
        colorAnimation.time = 0.3f + (heartPulse * healthSlider.value);

        UpdateMana();
        UpdateStamina();
        UpdateThirst();
    }
}
