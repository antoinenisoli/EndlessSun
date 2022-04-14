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
    bool hidden = true;

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
        /*Vector2 position = uiTarget.transform.position;
        float pivotX = position.x / Screen.width;
        float pivotY = position.y / Screen.height;
        rectTransform.pivot = new Vector2(pivotX, pivotY);
        rectTransform.position = position;*/

        hidden = false;
        tooltipContainer.SetActive(true);
        group.DOKill();
        group.DOFade(1, 0.1f);
        header.text = headerText;
        content.text = contentText;
    }

    public void Hide()
    {
        hidden = true;
        group.DOKill();
        group.DOFade(0, 0.1f).OnComplete(() =>
        {
            tooltipContainer.SetActive(false);
        });
    }

    private void Update()
    {
        if (!hidden)
        {
            Vector2 position = Input.mousePosition + Vector3.right * 150f;
            float pivotX = position.x / Screen.width;
            float pivotY = position.y / Screen.height;
            rectTransform.pivot = new Vector2(pivotX, pivotY);
            rectTransform.position = position;
        }
    }
}
