using UnityEngine;
using System.Collections;

// 원거리 공격하는 스켈레톤 처리

public class SkeletonMageCtrl : SkeletonCtrl {

    public GameObject firePos; // throwObject가 발사되는 위치

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        // 이 옵션이 없으면 FireObject에서 널 포인터 에러 발생
        // 초기에 적을 만들고 비활성화 하는 사이에 OnEnable이 호출되기 때문
        if (isFirstEnable)
        {
            isFirstEnable = false;
            return;
        }

        base.OnEnable();
        StartCoroutine(FireObject());
    }

    protected override void Update()
    {
        base.Update();
    }

    // 함수 : FireObject
    // 목적 : 공격 중인지 체크하고 탄 발사
    //        여기선 오브젝트 활성화만 하고 FirePosCtrl.cs에서 OnEnable로 처리
    IEnumerator FireObject()
    {
        while (!isDead)
        {
            if (isAttack)
            {
                if(!firePos.activeSelf)
                    firePos.SetActive(true);
            }
            else
            {
                if(firePos.activeSelf)
                    firePos.SetActive(false);
            }

            yield return null;
        }
    }
}
