using UnityEngine;
using System.Collections;

// 플레이어를 부드럽게 추적하는 카메라

public class CameraFollow : MonoBehaviour {

    public Transform target;            // 추적할 타겟
    public float smoothing = 5f;        // 어느정도 부드럽게 이동할 것인가

    Vector3 offset;                     // 카메라와 타겟간의 거리 벡터

    void Start()
    {
        offset = transform.position - target.position;
    }

    void Update()
    {
        Vector3 targetCamPos = target.position + offset;    // offset 만큼 떨어진 타겟의 위치
        // 타겟을 부드럽게 추적
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
}
