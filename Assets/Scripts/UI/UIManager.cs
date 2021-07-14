using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Image xpSlider;
    [SerializeField] Text currentLevelText;
    [SerializeField] SpriteColorAnimation colorAnimation;
    [SerializeField] int heartPulse;
    [SerializeField] Slider healthSlider, manaSlider, staminaSlider;

    private void Update()
    {
        currentLevelText.text = GameManager.Player.myXP.CurrentLevel + 1 + "";
        xpSlider.fillAmount = (float)GameManager.Player.myXP.CurrentXP / (float)GameManager.Player.myXP.NextLevelXP;
        healthSlider.value = GameManager.Player.health.CurrentHealth / GameManager.Player.health.MaxHealth;
        colorAnimation.time = 0.3f + (heartPulse * healthSlider.value);
    }
}
