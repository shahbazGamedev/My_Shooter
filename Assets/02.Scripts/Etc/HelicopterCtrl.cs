using UnityEngine;
using System.Collections;

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
        if(coll.CompareTag("STOPPER"))
        {
            isMoving = false;
        }
    }
}
