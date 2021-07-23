using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Image xpSlider;
    [SerializeField] Text currentLevelText;
    public static UIManager Instance;
    HUD[] allMenus;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        allMenus = FindObjectsOfType<HUD>();
    }

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        currentLevelText.text = GameManager.Player.myXP.CurrentLevel.index + 1 + "";
        float computeXP = (float)GameManager.Player.myXP.CurrentLevel.CurrentXP / (float)GameManager.Player.myXP.CurrentLevel.xpStep;
        xpSlider.DOFillAmount(computeXP, 0.15f);

        foreach (var item in allMenus)
            item.UpdateUI();
    }
}
