using UnityEngine;
using System.Collections;

// 탄막에 사용되는 스크립트
// 플레이어가 쏘는 총알과 MageSkeleton이 쏘는 원거리 탄막에 사용

public class BulletCtrl : MonoBehaviour {

    public int damage = 1;
    public float speed = 1000.0f;
    public float disableDelay = 1.0f;                   // 탄이 유지되는 최대 시간. 시간이 지나면 disable

    private bool hasTrail;
    private float trailDelay;

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
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
        StartCoroutine(DisableBullet());

        if(hasTrail)
            StartCoroutine(EnableTrail());
    }

    void OnDisable()
    {
        GetComponent<Rigidbody>().isKinematic = true;

        if (hasTrail)
            GetComponent<TrailRenderer>().enabled = false;
    }

    // 맵 오브젝트에 충돌하면 바로 사라지도록 함
    void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.CompareTag("MAP_OBJECT"))
        {
            gameObject.SetActive(false);
        }
    }

    // disableDelay 만큼 기다렸다 총알 삭제
    IEnumerator DisableBullet()
    {
        yield return new WaitForSeconds(disableDelay);

        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    // TrailRenderer가 바로 활성화되면 이미지 문제가 생기므로 초기화 되길 기다렸다가 활성화한다.
    // 적 탄은 호출되지 않음
    IEnumerator EnableTrail()
    {
        yield return new WaitForSeconds(trailDelay);
        GetComponent<TrailRenderer>().enabled = true;
    }
}
