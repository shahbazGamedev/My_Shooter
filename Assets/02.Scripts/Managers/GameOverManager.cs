using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

// 게임 오버 처리

public class GameOverManager : MonoBehaviour {

    public Text gameOverTxt;
    public Text gameOverScoreTxt;
    public InputField inputName;
    public Text debugTxt;

    public bool isScoreSave;

    public static GameOverManager instance = null;

    private Animator anim;
    private bool isEndClip = false;

    void Awake()
    {
        isScoreSave = false;

        anim = GetComponent<Animator>();
        instance = this;
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
    }

    // 함수 : EndGameOverClip()
    // 목적 : 게임 오버 애니메이션 클립이 종료되면 호출. 총 점수를 텍스트에 표시
    public void EndGameOverClip()
    {
        isEndClip = true;
        gameOverScoreTxt.text = "Total Score : " + ScoreManager.instance.totalScore;
    }

    // 함수 : OnClickBackTitleButton
    // 목적 : BackTitleButton 클릭 시 호출
    //        타이틀 화면으로 돌아간다
    public void OnClickBackTitleButton()
    {
        if(isEndClip)
            SceneManager.LoadScene("scTitle");
    }

    // 함수 : OnClickRankButton
    // 목적 : RankButton 클릭 시 호출
    //        사용자 이름과 점수를 웹 서버에 올려 점수 랭킹 등록
    public void OnClickRankButton()
    {
        if(string.IsNullOrEmpty(inputName.text))
        {
            debugTxt.text = "이름을 입력하세요";
            return;
        }
        else if(!isScoreSave)
        {
            // 웹 호스팅 mysql 데이터베이스에 점수 등록
            StartCoroutine(DBManager.instance.SaveScore(
                            inputName.text, ScoreManager.instance.totalScore, debugTxt));
        }
        else if(isScoreSave)
        {
            debugTxt.text = "이미 등록됐습니다";
        }
    }
}
