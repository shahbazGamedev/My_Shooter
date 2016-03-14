using UnityEngine;
using System.Collections;

// 플레이어가 유탄 발사기로 발사하는 유탄 처리

public class GrenadeCtrl : MonoBehaviour {

    public int damage = 10;                                     // 유탄의 데미지
    public float speed = 1500f;                                 // 유탄의 발사 속도
    public float disableDelay = 1f;                             // 유탄의 최대 유지 시간
    public float explosionRadius = 3f;                          // 폭발 반경

    public LayerMask enemyMask;                                 // 적만 폭발 데미지를 입도록 하기 위한 마스크
    public ParticleSystem explosionParticle;                    // 폭발 파티클

    void OnEnable()
    {
        GetComponent<Rigidbody>().isKinematic = false;                      // 물리 효과 활성화
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);      // z축 방향으로 발사
        StartCoroutine(DisableGrenade());

        explosionParticle.transform.parent = transform;                     // 파티클의 부모와 위치를 재설정
        explosionParticle.transform.position = transform.position;
    }

    void OnDisable()
    {
        GetComponent<Rigidbody>().isKinematic = true;
    }

    void OnTriggerEnter(Collider coll)
    {
        // 맵 오브젝트나 적과 충돌하면 폭발
        if (coll.gameObject.CompareTag("MAP_OBJECT") || coll.gameObject.CompareTag("ENEMY"))
        {
            Explosion();
        }
    }

    // 함수 : DisableGrenade
    // 목적 : disableDelay 만큼 기다렸다 오브젝트 비활성화
    IEnumerator DisableGrenade()
    {
        yield return new WaitForSeconds(disableDelay);

        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

    // 함수 : Explosion
    // 목적 : 유탄의 폭발 효과를 내고 반경 내의 적에게 데미지를 줌
    void Explosion()
    {
        // 폭발 범위에 들어온 적을 받아옴
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, enemyMask);

        // 폭발 범위에 들어온 적에게 데미지를 입힘
        for(int i = 0; i < colliders.Length; i++)
        {
            SkeletonHealth enemyHealth = colliders[i].GetComponent<SkeletonHealth>();

            if (!enemyHealth)
                continue;

            enemyHealth.TakeDamage(damage);
        }

        // 폭발 효과는 그 자리에 남아있어야 하므로 부모를 null로
        explosionParticle.transform.parent = null;
        explosionParticle.Stop();
        explosionParticle.Play();

        gameObject.SetActive(false);
    }
}
