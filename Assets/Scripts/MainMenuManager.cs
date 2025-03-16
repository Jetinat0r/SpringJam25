using DG.Tweening;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    public RectTransform panelContainer;
    [SerializeField]
    public RectTransform mainPanel;
    [SerializeField]
    public RectTransform levelSelectPanel;
    [SerializeField]
    public RectTransform settingsPanel;
    [SerializeField]
    public RectTransform instructionsPanel;
    [SerializeField]
    public RectTransform creditsPanel;

    [SerializeField]
    [Tooltip("Move time in seconds")]
    public float tweenMoveTime = 1f;
    [SerializeField]
    public Ease tweenEaseType = Ease.OutQuint;

    //Used to slide the UI around. Cached because I don't know how things work when they slide
    private Vector2 anchoredMainPanelPos;
    private Vector2 anchoredLevelSelectPanelPos;
    private Vector2 anchoredSettingsPanelPos;
    private Vector2 anchoredInstructionsPanelPos;
    private Vector2 anchoredCreditsPanelPos;

    private Tween curTween;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anchoredMainPanelPos = mainPanel.anchoredPosition;
        anchoredLevelSelectPanelPos = levelSelectPanel.anchoredPosition;
        anchoredSettingsPanelPos = settingsPanel.anchoredPosition;
        anchoredInstructionsPanelPos = instructionsPanel.anchoredPosition;
        anchoredCreditsPanelPos = creditsPanel.anchoredPosition;
    }

    public void MoveToMainMenu(bool _isXMove)
    {
        curTween?.Kill();
        curTween = panelContainer.DOAnchorPos(-anchoredMainPanelPos, tweenMoveTime).SetEase(tweenEaseType);
    }

    public void MoveToLevelSelectPanel()
    {
        curTween?.Kill();
        curTween = panelContainer.DOAnchorPos(-anchoredLevelSelectPanelPos, tweenMoveTime).SetEase(tweenEaseType);
    }

    public void MoveToSettingsPanel()
    {
        curTween?.Kill();
        curTween = panelContainer.DOAnchorPos(-anchoredSettingsPanelPos, tweenMoveTime).SetEase(tweenEaseType);
    }

    public void MoveToInstructionsPanel()
    {
        instructionsPanel.gameObject.SetActive(true);
        creditsPanel.gameObject.SetActive(false);

        curTween?.Kill();
        curTween = panelContainer.DOAnchorPos(-anchoredInstructionsPanelPos, tweenMoveTime).SetEase(tweenEaseType);
    }

    public void MoveToCreditsPanel()
    {
        instructionsPanel.gameObject.SetActive(false);
        creditsPanel.gameObject.SetActive(true);

        curTween?.Kill();
        curTween = panelContainer.DOAnchorPos(-anchoredCreditsPanelPos, tweenMoveTime).SetEase(tweenEaseType);
    }
}
