using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DBManager : MonoBehaviour {

    // 싱글턴 인스턴스
    public static DBManager instance = null;

    // php 파일이 있는 주소
    private const string urlSave = "http://zerohour.dothome.co.kr/save_score.php";            // 점수 저장
    private const string urlGetRank = "http://zerohour.dothome.co.kr/get_score_rank.php";     // 랭킹 불러오기

	void Awake()
    {
        instance = this;
    }

    // 함수 : SaveScore
    // 목적 : 웹 서버에 랭킹 정보를 등록하고 결과를 debugTxt에 출력
    //        GameOverManager의 OnClickRankButton 함수에서 호출
    public IEnumerator SaveScore(string name, int score, Text debugTxt)
    {
        // POST 방식으로 전달하기 위한 폼
        WWWForm form = new WWWForm();

        // 파라미터 설정
        form.AddField("name", name);
        form.AddField("score", score);

        var www = new WWW(urlSave, form);

        // 완료 시점까지 대기
        yield return www;

        if(string.IsNullOrEmpty(www.error))
        {
            debugTxt.text = www.text;

            // 한번만 점수 등록할 수 있도록 등록이 성공하면 변수를 true로 바꾼다
            GameOverManager.instance.isScoreSave = true;
        }
        else
        {
            debugTxt.text = www.error;  // 에러 발생 시 에러 메시지 표시
        }
    }

    // 함수 : GetRankList
    // 목적 : 웹 서버의 DB 테이블에서 랭킹 정보를 불러오고 이를 이용해 리스트 업데이트 함수를 호출
    //        GameOverManager의 OnClickRankButton 함수에서 호출
    public IEnumerator GetRankList()
    {
        var www = new WWW(urlGetRank);

        yield return www;

        if(string.IsNullOrEmpty(www.error))
        {
            // 불러온 랭킹 정보를 사용해 리스트를 업데이트 하도록 함수 호출
            RankListManager.instance.RankListUpdate(www.text);
        }
        else
        {
            Debug.Log(www.error);
        }
    }
}
