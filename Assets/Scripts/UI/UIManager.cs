using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] CanvasGroup levelUpPanel;
    [SerializeField] Text currentLevelText;

    [SerializeField] CanvasGroup interactTooltip;
    [SerializeField] Text interactTooltipText;

    [SerializeField] Image xpSlider;
    HUD[] allMenus;
    Camera mainCam;

    [HideInInspector] public PlayerInventoryUI inventoryUI;

    private void Awake()
    {
        Singleton();
        allMenus = FindObjectsOfType<HUD>();
        mainCam = Camera.main;
        inventoryUI = FindObjectOfType<PlayerInventoryUI>();
    }

    private void Start()
    {
        levelUpPanel.DOFade(0f, 0f);
        UpdateUI();
    }

    void Singleton()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void UpdateUI()
    {
        currentLevelText.text = GameManager.Player.myXP.CurrentLevel.index + 1 + "";
        float computeXP = (float)GameManager.Player.myXP.CurrentLevel.CurrentXP / (float)GameManager.Player.myXP.CurrentLevel.xpStep;
        xpSlider.DOFillAmount(computeXP, 0.15f);

        foreach (var item in allMenus)
            item.UpdateUI();
    }

    public void ShowPickUp(Interactable item)
    {
        interactTooltip.gameObject.SetActive(item != null);
        if (item)
        {
            interactTooltip.transform.position = mainCam.WorldToScreenPoint(item.transform.position - Vector3.up * 0.1f);
            interactTooltipText.text = item.ToString();
        }
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
