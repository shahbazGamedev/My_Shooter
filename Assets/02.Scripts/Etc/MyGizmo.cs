using UnityEngine;
using System.Collections;

public class MyGizmo : MonoBehaviour
{
    public Color color = Color.yellow;
    public float radius = 0.1f;

    void OnDrawGizmos()
    {
        // 기즈모 색상 설정
        Gizmos.color = color;
        // 구체 모양의 기즈모 생성. 인자는 (생성 위치, 반지름)
        Gizmos.DrawSphere(transform.position, radius);
    }
}
