using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 점수 표기 처리

public class ScoreManager : MonoBehaviour {

    public static int totalScore;

    private Text scoreText;

	void Awake()
    {
        scoreText = GetComponent<Text>();
        totalScore = 0;
    }

    void Update()
    {
        scoreText.text = "Score " + totalScore;
    }
}
