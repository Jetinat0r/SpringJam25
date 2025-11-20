using JetEngine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FixedScrollArea : ScrollRect
{
    [SerializeField]
    private ScrollRect scrollRect;

    [SerializeField]
    private int numScrollPositions = 5;
    [SerializeField]
    private float elementHeight = 16f;
    [SerializeField]
    private float elementPadding = 4f;

    private float anchoredScrollDelta;
    public int scrollPos = 0;

    protected override void Start()
    {
        base.Start();

        anchoredScrollDelta = elementHeight + elementPadding;
        content.anchoredPosition = content.anchoredPosition.NewY(0f);
    }

    public override void OnScroll(PointerEventData data)
    {
        if (!IsActive())
        {
            return;
        }

        float _delta = -data.scrollDelta.y;
        if (Mathf.Abs(_delta) < Mathf.Abs(scrollSensitivity))
        {
            //Didn't scroll far/fast enough; return
            return;
        }

        scrollPos += Mathf.RoundToInt(Mathf.Sign(_delta));
        UpdateScroll();
    }

    public void UpdateScroll()
    {
        scrollPos = Mathf.Clamp(scrollPos, 0, numScrollPositions - 1);

        content.anchoredPosition = content.anchoredPosition.NewY(scrollPos * anchoredScrollDelta);
        Canvas.ForceUpdateCanvases();
    }
}
