using UnityEngine;
using UnityEngine.UI;

public class TabButtonBehavior : MonoBehaviour
{
    [SerializeField]
    public Button tabButton;
    [SerializeField]
    public TabButtonBehavior[] tabs;

    [SerializeField]
    public GameObject activeTabDisplay;
    [SerializeField]
    public GameObject tabContent;
    [SerializeField]
    public Selectable firstNavigableElement;

    [SerializeField]
    private SoundPlayer soundPlayer;

    public void ShowTab()
    {
        //Don't activate tab if already active
        if (activeTabDisplay.activeSelf)
        {
            return;
        }

        foreach (TabButtonBehavior t in tabs)
        {
            t.HideTab();

            //Make all tabs down nav into the active menu
            Navigation _nav = t.tabButton.navigation;
            _nav.selectOnDown = firstNavigableElement;
            t.tabButton.navigation = _nav;
        }

        soundPlayer.PlaySound("UI.Select");
        tabContent.SetActive(true);
        activeTabDisplay.SetActive(true);
    }

    public void HideTab()
    {
        Navigation _nav = tabButton.navigation;
        _nav.selectOnDown = null;
        tabButton.navigation = _nav;

        tabContent.SetActive(false);
        activeTabDisplay.SetActive(false);
    }
}
