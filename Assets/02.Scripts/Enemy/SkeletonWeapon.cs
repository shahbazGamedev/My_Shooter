using UnityEngine;
using System.Collections;

// 몬스터의 근접 무기 스크립트

public class SkeletonWeapon : MonoBehaviour {

    public int damage = 10; // 무기 공격력

    private CapsuleCollider weaponColl;

    void Awake()
    {
        weaponColl = GetComponent<CapsuleCollider>();
    }

    // 함수 : EnableCollider
    // 목적 : 무기 컬라이더를 활성화 또는 비활성화
    //        SkeletonWarriorCtrl에서 호출
    public void EnableCollider(bool active)
    {
        weaponColl.enabled = active;
    }
}
