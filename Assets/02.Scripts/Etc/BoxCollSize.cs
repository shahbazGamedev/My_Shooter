using UnityEngine;
using System.Collections;

// RectTransform의 Anchors 값과 화면 크기에 비례해 BoxCollider 크기를 조정

public class BoxCollSize : MonoBehaviour {

    private float anchorX, anchorY;

    private BoxCollider coll;
    private RectTransform rect;

    void Awake()
    {
        coll = GetComponent<BoxCollider>();
        rect = GetComponent<RectTransform>();

        anchorX = rect.anchorMax.x - rect.anchorMin.x;
        anchorY = rect.anchorMax.y - rect.anchorMin.y;

        coll.size = new Vector3(Screen.width * anchorX, Screen.height * anchorY);
    }
}
