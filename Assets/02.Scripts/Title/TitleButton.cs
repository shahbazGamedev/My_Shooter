using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

// 타이틀 씬의 버튼 함수 스크립트

public class TitleButton : MonoBehaviour {

    public GameObject gameMenu;
    public GameObject tutorialPanel;
    public GameObject rankPanel;

    // 함수 : OnClickGameStart
    // 목적 : 게임 시작 버튼 함수. 메인 씬을 로드
    public void OnClickGameStart()
    {
        SceneManager.LoadScene("scMain");
    }

    // 함수 : OnClickTutorial
    // 목적 : 게임 설명 버튼 함수. 메뉴를 비활성화 하고 튜토리얼 패널을 활성화
    public void OnClickTutorial()
    {
        gameMenu.SetActive(false);
        tutorialPanel.SetActive(true);
    }

    // 함수 : OnClickQuit
    // 목적 : 종료 버튼 함수. 프로그램을 종료
    public void OnClickQuit()
    {
        Application.Quit();
    }

    // 함수 : OnClickBack
    // 목적 : 돌아가기 버튼 함수. OnClickTutorial의 반대 기능
    public void OnClickBack()
    {
        gameMenu.SetActive(true);
        tutorialPanel.SetActive(false);
        rankPanel.SetActive(false);
    }

    // 함수 : OnClickRank
    // 목적 : 온라인 랭킹 버튼 함수. 랭킹 패널을 활성화
    //        DBManager를 통해 웹 서버에서 스코어 랭킹 정보를 요청
    public void OnClickRank()
    {
        gameMenu.SetActive(false);
        rankPanel.SetActive(true);

        StartCoroutine(DBManager.instance.GetRankList());
    }
}
