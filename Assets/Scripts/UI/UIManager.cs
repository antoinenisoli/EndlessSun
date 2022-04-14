using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] CanvasGroup levelUpPanel;
    [SerializeField] Image blackScreenImage;
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

        levelUpPanel.DOFade(0f, 0f);
        blackScreenImage.DOFade(0f, 0f);
    }

    private IEnumerator Start()
    {
        EventManager.Instance.onPlayerSleep.AddListener(Sleep);
        yield return new WaitForEndOfFrame();     
        UpdateUI();
    }

    void Singleton()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Sleep()
    {
        BlackScreen(1f);
        StartCoroutine(AwakePlayer(2f));
    }

    IEnumerator AwakePlayer(float delay)
    {
        yield return new WaitForSeconds(delay);
        EventManager.Instance.onPlayerAwake.Invoke();
    }

    public void UpdateUI()
    {
        if (!GameManager.Player)
            return;

        if (PlayerController2D.xpManager)
        {
            currentLevelText.text = PlayerController2D.xpManager.CurrentLevel.index + 1 + "";
            float computeXP = PlayerController2D.xpManager.ComputeXP();
            xpSlider.DOFillAmount(computeXP, 0.15f);
        }

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

    public void BlackScreen(float alpha = 0.5f, float duration = 1f)
    {
        blackScreenImage.gameObject.SetActive(true);
        Sequence fadeSeq = DOTween.Sequence();
        fadeSeq.SetUpdate(true);
        fadeSeq.Append(blackScreenImage.DOFade(alpha, 0.5f));
        fadeSeq.AppendInterval(duration);
        fadeSeq.SetLoops(2, LoopType.Yoyo);
        fadeSeq.OnComplete(() => 
        { 
            blackScreenImage.gameObject.SetActive(false); 
        });
    }

    public void LevelUp()
    {
        BlackScreen(0.5f, 0.8f);
        Sequence fadeSeq = DOTween.Sequence();
        fadeSeq.SetUpdate(true);
        fadeSeq.Join(levelUpPanel.transform.DOScale(Vector3.one * 1f, 0.7f));
        fadeSeq.AppendInterval(1f);
        fadeSeq.SetLoops(2, LoopType.Yoyo);
    }
}
