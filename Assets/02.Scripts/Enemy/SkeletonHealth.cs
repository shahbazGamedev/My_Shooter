using UnityEngine;
using System.Collections;

public class SkeletonHealth : MonoBehaviour {

    public int maxHp;
    public int currentHp;

    private SkeletonCtrl skelCtrl;

    void Awake()
    {
        skelCtrl = GetComponent<SkeletonCtrl>();
    }

    void OnEnable()
    {
        currentHp = maxHp;
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("BULLET"))
        {
            TakeDamage(coll.gameObject.GetComponent<BulletCtrl>().damage);
            CreateFlare(coll.transform.position);

            coll.gameObject.SetActive(false);
        }
    }

    // 피격 처리. GrenadeCtrl에서 호출하기 위해 public 선언
    public void TakeDamage(int damage)
    {
        // 죽었으면 처리 안함
        if (skelCtrl.isDead)
            return;

        currentHp -= damage;

        if(currentHp <= 0)
        {
            GetComponent<SkeletonCtrl>().ActionDie();
        }
    }

    // 피격시 맞는 이펙트를 생성하는 하는 함수
    void CreateFlare(Vector3 pos)
    {
        foreach (GameObject flare in BulletManager.instance.flarePool)
        {
            // 활성화 되지 않은 파티클을 활성화
            if (!flare.activeSelf)
            {
                flare.transform.position = transform.position;
                flare.SetActive(true);
                break;
            }
        }
    }
}
