using UnityEngine;
using System.Collections;

// 근접 공격하는 스켈레톤 처리

public class SkeletonWarriorCtrl : SkeletonCtrl {

    private SkeletonWeapon weaponScript;

    protected override void Awake ()
    {
        base.Awake();
        weaponScript = GetComponentInChildren<SkeletonWeapon>();
    }

    protected override void OnEnable()
    {
        // 이 옵션이 없으면 EnableWeaponCollider에서 널 포인터 에러 발생
        // 초기에 적을 만들고 비활성화 하는 사이에 OnEnable이 호출되기 때문
        if (isFirstEnable)
        {
            isFirstEnable = false;
            return;
        }

        base.OnEnable();
        StartCoroutine(EnableWeaponCollider());
    }
	
	protected override void Update ()
    {
        base.Update();
	}

    IEnumerator EnableWeaponCollider()
    {
        while (!isDead)
        {
            if (isAttack)
            {
                weaponScript.EnableCollider(true);
            }
            else
            {
                weaponScript.EnableCollider(false);
            }

            yield return null;
        }

        // 사망 시 무기 컬라이더 비활성화
        if(isDead)
            weaponScript.EnableCollider(false);
    }
}
