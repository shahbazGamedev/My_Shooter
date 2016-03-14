using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

// 게임 오버 처리

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
        // 게임 오버 애니메이션 실행
        if(GameManager.instance.isGameOver)
        {
            anim.SetTrigger("GameOver");
        }

        // 게임 클리어 애니메이션 실행
        if(GameManager.instance.isGameClear)
        {
            gameOverTxt.text = "Game Clear";
            anim.SetTrigger("GameClear");
        }

        // 애니메이션 재생이 끝났고 화면을 터치하면 타이틀 씬으로 이동
        if(isEndClip && ( (Input.touchCount > 0) || Input.GetMouseButtonDown(0) ))
        {
            SceneManager.LoadScene("scTitle");
        }
    }

    // 함수 : EndGameOverClip()
    // 목적 : 게임 오버 애니메이션 클립이 종료되면 호출. 총 점수를 텍스트에 표시
    public void EndGameOverClip()
    {
        isEndClip = true;
        gameOverScoreTxt.text = "Total Score : " + ScoreManager.instance.totalScore;
    }
}
