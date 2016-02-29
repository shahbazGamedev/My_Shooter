using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour {

    public Text gameOverTxt;
    public Text gameOverScoreTxt;

    private Animator anim;
    private bool isEndClip = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if(GameManager.instance.isGameOver)
        {
            anim.SetTrigger("GameOver");
        }

        if(GameManager.instance.isGameClear)
        {
            gameOverTxt.text = "Game Clear";
            anim.SetTrigger("GameClear");
        }

        if(isEndClip && ( (Input.touchCount > 0) || Input.GetMouseButtonDown(0) ))
        {
            SceneManager.LoadScene("scTitle");
        }
    }

    public void EndGameOverClip()
    {
        isEndClip = true;
        gameOverScoreTxt.text = "Total Score : " + ScoreManager.totalScore;
    }
}
