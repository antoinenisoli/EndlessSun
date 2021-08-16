using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem Instance;
    CanvasGroup group;
    RectTransform rectTransform;
    [SerializeField] Text header;
    [SerializeField] Text content;
    [SerializeField] GameObject tooltipContainer;

    private void Awake()
    {
        Singleton();
        rectTransform = GetComponent<RectTransform>();
        group = GetComponent<CanvasGroup>();
        tooltipContainer.SetActive(false);
    }

    void Singleton()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Show(string headerText, string contentText, RectTransform uiTarget)
    {
        rectTransform.position = uiTarget.position + Vector3.right * 100;
        tooltipContainer.SetActive(true);
        group.DOKill();
        group.DOFade(1, 0.2f);
        header.text = headerText;
        content.text = contentText;
    }

    public void Hide()
    {
        group.DOKill();
        group.DOFade(0, 0.2f).OnComplete(() =>
        {
            tooltipContainer.SetActive(false);
        });
    }
}
