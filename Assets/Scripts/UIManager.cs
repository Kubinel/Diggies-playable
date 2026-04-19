using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject tutorialScreen;

    private void OnEnable()
    {
        ClickToMove.Tutorial += ShowTutorial;
        ClickToMove.TutorialDone += HideAll;
        GridManager.Win += ShowWin;
    }

    private void OnDisable()
    {
        ClickToMove.Tutorial -= ShowTutorial;
        ClickToMove.TutorialDone -= HideAll;
        GridManager.Win -= ShowWin;
    }

    private void ShowTutorial()
    {
        winScreen.SetActive(false);
        tutorialScreen.SetActive(true);
    }

    private void HideAll()
    {
        winScreen.SetActive(false);
        tutorialScreen.SetActive(false);
    }

    private void ShowWin()
    {
        winScreen.SetActive(true);
        tutorialScreen.SetActive(false);
    }
}