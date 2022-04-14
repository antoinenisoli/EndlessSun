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

    IEnumerator Start()
    {
        yield return null;
        if (GameManager.Player)
        {
            yield return new WaitForEndOfFrame();
            UpdateUI();
        }
    }

    void UpdateMana()
    {
        float compute = PlayerSurvival.Instance.Hunger.MaxValue - PlayerSurvival.Instance.Hunger.CurrentValue;
        if (float.IsNaN(compute) || float.IsInfinity(compute))
            return;

        hungerSlider.maxValue = PlayerSurvival.Instance.Hunger.MaxValue;
        hungerSlider.value = compute;
        if (PlayerMagic.Mana != null)
        {
            manaSlider.maxValue = PlayerMagic.Mana.MaxValue;
            manaSlider.value = PlayerMagic.Mana.CurrentValue;
        }
    }

    void UpdateStamina()
    {
        float compute = PlayerSurvival.Instance.Energy.MaxValue - PlayerSurvival.Instance.Energy.CurrentValue;
        if (float.IsNaN(compute) || float.IsInfinity(compute))
            return;

        energySlider.maxValue = PlayerSurvival.Instance.Energy.MaxValue;
        energySlider.value = compute;
        if (PlayerCombat.Stamina != null)
        {
            staminaSlider.maxValue = PlayerCombat.Stamina.MaxValue;
            staminaSlider.value = PlayerCombat.Stamina.CurrentValue;
        }
    }

    void UpdateHealth()
    {
        float compute = GameManager.Player.Health.Coeff();
        if (float.IsNaN(compute) || float.IsInfinity(compute))
            return;

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
