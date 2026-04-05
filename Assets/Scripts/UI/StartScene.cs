using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel, howToPlay, tips, mechanics;
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Tutorial()
    {
        tutorialPanel.SetActive(true);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void BackButton()
    {
        tutorialPanel.SetActive(false);
    }

    public void MechanicsButton()
    {
        mechanics.SetActive(true);
        howToPlay.SetActive(false);
        tips.SetActive(false);
    }

    public void TipsButton()
    {
        tips.SetActive(true);
        mechanics.SetActive(false);
        howToPlay.SetActive(false);
    }

    public void HowToPlayButton()
    {
        howToPlay.SetActive(true);
        tips.SetActive(false);
        mechanics.SetActive(false);
    }
}