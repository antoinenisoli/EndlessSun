using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Image xpSlider;
    [SerializeField] Text currentLevelText;
    [SerializeField] SpriteColorAnimation colorAnimation;
    [SerializeField] int heartPulse;
    [SerializeField] Slider healthSlider, manaSlider, staminaSlider;
    public static UIManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        currentLevelText.text = GameManager.Player.myXP.CurrentLevel.index + 1 + "";
        xpSlider.DOFillAmount((float)GameManager.Player.myXP.CurrentLevel.CurrentXP / (float)GameManager.Player.myXP.CurrentLevel.xpStep, 0.15f);
        healthSlider.DOValue(GameManager.Player.health.CurrentHealth / GameManager.Player.health.MaxHealth, 1f);
        colorAnimation.time = 0.3f + (heartPulse * healthSlider.value);
    }
}
