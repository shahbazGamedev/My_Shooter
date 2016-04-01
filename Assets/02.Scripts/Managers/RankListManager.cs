using UnityEngine;
using System.Collections;
using SimpleJSON;

public class RankListManager : MonoBehaviour {

    public GameObject rankItemPrefab;       // 랭크 정보 아이템 프리팹
    public RectTransform scrollContents;    // 랭크 정보를 포함하는 스크롤 영역

    public static RankListManager instance = null; // 싱글턴 인스턴스

    private const int padding = 60;         // 랭크 아이템 사이의 간격

	void Awake()
    {
        instance = this;
    }

    // 함수 : RankListUpdate
    // 목적 : DBManager의 GetRankList 함수에서 웹 서버의 랭킹 정보를 이용해 호출.
    //        JSON 포맷의 랭킹 정보를 받아 온라인 랭킹 리스트를 만들어 출력
    public void RankListUpdate(string strJsonData)
    {        
        var N = JSON.Parse(strJsonData); // JSON 파싱

        for (int i = 0; i < N.Count; i++)
        {
            string ranking = (i + 1).ToString();        // 랭크
            string name = N[i]["name"].ToString();      // 이름
            string score = N[i]["score"].ToString();    // 점수

            // 랭크 아이템을 생성하고 부모를 스크롤 영역으로 설정
            GameObject rankItem = (GameObject)Instantiate(rankItemPrefab);
            rankItem.transform.SetParent(scrollContents.transform, false);

            // 새로운 아이템의 위치를 지정하고 텍스트 초기화
            rankItem.GetComponent<RectTransform>().localPosition += new Vector3(0, -padding * i, 0);
            rankItem.GetComponent<RankItemScript>().SetTexts(ranking, name, score);

            scrollContents.sizeDelta += new Vector2(0, padding); // 새로 아이템이 만들어진 만큼 스크롤 영역 확장
        }
    }
}
