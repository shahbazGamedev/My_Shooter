using UnityEngine;
using System.Collections;

// 아이템의 공통된 동작을 처리

public class ItemCtrl : MonoBehaviour {

    public float disableDelay;

    void OnEnable()
    {
        RandomRotation();               // 활성화되면 랜덤하게 회전
        StartCoroutine(DisableItem());
    }

    // 랜덤하게 y축 기준으로 회전
    void RandomRotation()
    {
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
    }

    IEnumerator DisableItem()
    {
        yield return new WaitForSeconds(disableDelay);

        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }
}
