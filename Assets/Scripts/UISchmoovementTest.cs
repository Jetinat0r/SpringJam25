using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class UISchmoovementTest : MonoBehaviour
{
    [SerializeField]
    public RectTransform panelContainer;
    [SerializeField]
    public Vector2 referenceScreenSize = new Vector2(320f, 288f);

    [SerializeField]
    public Ease moveEaseType = Ease.Unset;

    private Vector2 anchoredPos = Vector2.zero;
    private Tween curTween;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            anchoredPos.x += referenceScreenSize.x;

            curTween?.Kill();
            curTween = panelContainer.DOAnchorPos(anchoredPos, 1f).SetEase(moveEaseType);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            anchoredPos.y += referenceScreenSize.y;

            curTween?.Kill();
            curTween = panelContainer.DOAnchorPos(anchoredPos, 1f).SetEase(moveEaseType);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            anchoredPos.x -= referenceScreenSize.x;

            curTween?.Kill();
            curTween = panelContainer.DOAnchorPos(anchoredPos, 1f).SetEase(moveEaseType);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            anchoredPos.y -= referenceScreenSize.y;

            curTween?.Kill();
            curTween = panelContainer.DOAnchorPos(anchoredPos, 1f).SetEase(moveEaseType);
        }
    }
}
