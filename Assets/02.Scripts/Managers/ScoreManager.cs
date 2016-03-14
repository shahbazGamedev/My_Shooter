using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 점수 표기 처리

public class ScoreManager : MonoBehaviour {

    public static ScoreManager instance = null;

    public int totalScore;

    private Text scoreText;

	void Awake()
    {
        instance = this;
        scoreText = GetComponent<Text>();
        totalScore = 0;
    }

    // 함수 : GetScore
    // 목적 : 점수 획득. 몬스터가 호출하기 위해 public
    public void GetScore(int score)
    {
        totalScore += score;
        scoreText.text = "Score " + totalScore;
    }
}
