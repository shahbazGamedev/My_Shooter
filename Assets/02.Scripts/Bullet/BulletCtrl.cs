using UnityEngine;
using System.Collections;

// 탄막에 사용되는 스크립트
// 플레이어가 쏘는 총알과 MageSkeleton이 쏘는 원거리 탄막에 사용

public class BulletCtrl : MonoBehaviour {

    public int damage = 1;                              // 탄의 데미지
    public float speed = 1000.0f;                       // 탄이 날아가는 속도
    public float disableDelay = 1.0f;                   // 탄이 유지되는 최대 시간. 시간이 지나면 disable

    private bool hasTrail;                              // 탄 오브젝트에 Trail Renderer가 있는지 여부
    private float trailDelay;                           // Trail Renderer가 유지되는 시간

    // 플레이어 총알은 TrailRenderer가 있어서 이미지 초기화를 위해 trailDelay가 필요
    // 적이 쓰는 탄은 TrailRenderer가 없으므로 TrailRenderer에 접근하지 않도록 한다. 
    void Awake()
    {
        if (GetComponent<TrailRenderer>() != null)
        {
            hasTrail = true;
            trailDelay = GetComponent<TrailRenderer>().time;
        }
        else
            hasTrail = false;
    }
	
	void OnEnable ()
    {
        GetComponent<Rigidbody>().isKinematic = false;                      // 물리 효과 활성화
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);      // z축 방향으로 발사

        StartCoroutine(DisableBullet());
        if(hasTrail)
            StartCoroutine(EnableTrail());
    }

    void OnDisable()
    {
        GetComponent<Rigidbody>().isKinematic = true;                      // 물리 효과 비활성화

        if (hasTrail)
            GetComponent<TrailRenderer>().enabled = false;
    }

    
    void OnTriggerEnter(Collider coll)
    {
        // 맵 오브젝트에 충돌하면 바로 사라지도록 함
        if (coll.gameObject.CompareTag("MAP_OBJECT"))
        {
            gameObject.SetActive(false);
        }
    }

    // 함수 : DisableBullet
    // 목적 : disableDelay 만큼 기다렸다 오브젝트 비활성화
    IEnumerator DisableBullet()
    {
        yield return new WaitForSeconds(disableDelay);

        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    // 함수 : EnableTrail
    // 목적 : Trail Renderer가 바로 활성화되면 이미지 문제가 생기므로 딜레이 만큼 기다렸다가 활성화.
    //        적 탄은 TrailRenderer가 없어 호출되지 않음
    IEnumerator EnableTrail()
    {
        yield return new WaitForSeconds(trailDelay);
        GetComponent<TrailRenderer>().enabled = true;
    }
}
