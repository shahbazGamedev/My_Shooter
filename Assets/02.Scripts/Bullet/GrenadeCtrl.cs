using UnityEngine;
using System.Collections;

// 플레이어가 유탄 발사기로 발사하는 유탄 처리

public class GrenadeCtrl : MonoBehaviour {

    public int damage = 10;
    public float speed = 1500f;
    public float disableDelay = 1f;
    public float explosionRadius = 3f;
    public float explosionForce = 1000f;

    public LayerMask enemyMask;
    public ParticleSystem explosionParticle;

    void OnEnable()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
        StartCoroutine(DisableGrenade());

        explosionParticle.transform.parent = transform;
        explosionParticle.transform.position = transform.position;
    }

    void OnDisable()
    {
        GetComponent<Rigidbody>().isKinematic = true;
    }

    // 맵 오브젝트나 적과 충돌하면 폭발
    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("MAP_OBJECT") || coll.gameObject.CompareTag("ENEMY"))
        {
            Explosion();
        }
    }

    // disableDelay 만큼 기다렸다 유탄 삭제
    IEnumerator DisableGrenade()
    {
        yield return new WaitForSeconds(disableDelay);

        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }

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
