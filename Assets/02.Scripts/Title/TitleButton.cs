using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleButton : MonoBehaviour {

    public GameObject gameMenu;
    public GameObject tutorialPanel;

    public void OnClickGameStart()
    {
        SceneManager.LoadScene("scMain");
    }

    public void OnClickTutorial()
    {
        gameMenu.SetActive(false);
        tutorialPanel.SetActive(true);
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }

    public void OnClickBack()
    {
        gameMenu.SetActive(true);
        tutorialPanel.SetActive(false);
    }
}
