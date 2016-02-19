using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleButton : MonoBehaviour {

    public void OnClickGameStart()
    {
        SceneManager.LoadScene("scMain");
    }
}
