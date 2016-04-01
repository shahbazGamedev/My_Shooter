using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RankItemScript : MonoBehaviour {

    public Text rankTxt;
    public Text nameTxt;
    public Text scoreTxt;

    // 함수 : SetTexts
    // 목적 : 랭킹 정보 텍스트를 설정
    public void SetTexts(string ranking, string name, string score)
    {
        rankTxt.text = ranking;
        nameTxt.text = name;
        scoreTxt.text = score;
    }
}
