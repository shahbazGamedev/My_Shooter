using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour {

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

        if(isEndClip && (Input.touchCount > 0 || Input.GetMouseButtonDown(0)))
        {
            SceneManager.LoadScene("scTitle");
        }
    }

    public void EndGameOverClip()
    {
        isEndClip = true;
    }
}
