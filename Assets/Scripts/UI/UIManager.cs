using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] CanvasGroup levelUpPanel;
    [SerializeField] Image xpSlider;
    [SerializeField] Text currentLevelText;
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
        levelUpPanel.DOFade(0f, 0f);
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

    public void LevelUp()
    {
        Sequence fadeSeq = DOTween.Sequence();
        fadeSeq.SetUpdate(true);
        fadeSeq.Append(levelUpPanel.DOFade(1f, 0.5f));
        fadeSeq.Join(levelUpPanel.transform.DOScale(Vector3.one * 1f, 0.7f));
        fadeSeq.AppendInterval(1f);
        fadeSeq.SetLoops(2, LoopType.Yoyo);
    }
}
