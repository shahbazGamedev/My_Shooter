using UnityEngine;
using System.Collections;

public class SkeletonWeapon : MonoBehaviour {

    public int damage = 10;

    private CapsuleCollider weaponColl;

    void Awake()
    {
        weaponColl = GetComponent<CapsuleCollider>();
    }

    public void EnableCollider(bool active)
    {
        weaponColl.enabled = active;
    }
}
