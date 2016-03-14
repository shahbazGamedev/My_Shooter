using UnityEngine;
using System.Collections;

// 0시에 등장하는 헬리콥터 스크립트

public class HelicopterCtrl : MonoBehaviour {

    public float movSpeed;

    private bool isMoving;

	void OnEnable()
    {
        isMoving = true;
    }

    void Update()
    {
        if (isMoving)
        {
            movSpeed -= Time.deltaTime; // 가속도를 적용
            transform.Translate(movSpeed * -transform.up * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        // 헬기장의 STOPPER와 닿으면 정지
        if(coll.CompareTag("STOPPER"))
        {
            isMoving = false;
        }
    }
}
