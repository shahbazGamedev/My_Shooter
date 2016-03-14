using UnityEngine;
using System.Collections;

// 몬스터의 체력 관련 처리 스크립트

public class SkeletonHealth : MonoBehaviour {

    public int defaultMaxHp;                        // 최초의 최대 체력
    public int maxHp;                               // 현재 최대 체력 (게임 레벨이 올라감에 따라 증가)
    public int currentHp;                           // 현재 체력

    private bool isFirstPowerUp = false;            // 첫번째 체력 강화가 되었는지
    private bool isSecondPowerUp = false;           // 두번째 체력 강화가 되었는지

    private SkeletonCtrl skelCtrl;

    void Awake()
    {
        skelCtrl = GetComponent<SkeletonCtrl>();
    }

    void OnEnable()
    {
        // 게임 레벨 3 이상, 첫번째 체력 강화가 안됐다면 최초 체력 기준에서 강화
        if (GameManager.instance.gameLevel >= 3 && !isFirstPowerUp) {
            maxHp = defaultMaxHp + GameManager.instance.enemyHpUp;
            isFirstPowerUp = true;
        }

        // 게임 레벨 5 이상, 두번째 체력 강화가 안됐다면 최초 체력 기준에서 더 강화
        if (GameManager.instance.gameLevel >= 5 && !isSecondPowerUp)
        {
            maxHp = defaultMaxHp + GameManager.instance.enemyHpUp * 2;
            isSecondPowerUp = true;
        }

        currentHp = maxHp;
    }

    void OnTriggerEnter(Collider coll)
    {
        // 피탄 시 데미지 입고 피격 파티클 생성
        if (coll.gameObject.CompareTag("BULLET"))
        {
            TakeDamage(coll.gameObject.GetComponent<BulletCtrl>().damage);
            CreateFlare(coll.transform.position);

            coll.gameObject.SetActive(false);
        }
    }

    // 함수 : TakeDamage
    // 목적 : 피격 처리. GrenadeCtrl에서 호출하기 위해 public 선언
    public void TakeDamage(int damage)
    {
        // 죽었으면 처리 안함
        if (skelCtrl.isDead)
            return;

        currentHp -= damage;    // 체력 감소

        if(currentHp <= 0)
        {
            GetComponent<SkeletonCtrl>().ActionDie();
        }
    }

    // 함수 : CreateFlare
    // 목적 : 피격시 파티클을 생성
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
