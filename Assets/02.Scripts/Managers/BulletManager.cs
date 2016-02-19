using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 탄막을 미리 생성해 풀에 저장하는 작업을 처리

public class BulletManager : MonoBehaviour {

    public int maxNormalBullet = 10;
    public int maxMachineGunBullet = 20;
    public int maxGrenade = 3;
    public int maxDagger = 50;
    public int maxFlare = 20;

    public GameObject normalBulletPrefab;
    public GameObject machineGunBulletPrefab;
    public GameObject grenadePrefab;
    public GameObject daggerPrefab;
    public GameObject flarePrefab;

    public List<GameObject> normalBulletPool = new List<GameObject>();              // 기본 탄환을 미리 저장하는 풀
    public List<GameObject> machineGunBulletPool = new List<GameObject>();          // 머신건 탄환을 미리 저장하는 풀
    public List<GameObject> grenadePool = new List<GameObject>();                   // 유탄을 미리 저장하는 풀
    public List<GameObject> daggerPool = new List<GameObject>();                    // 적이 날리는 단검을 미리 저장하는 풀
    public List<GameObject> flarePool = new List<GameObject>();                     // 탄 명중시 피탄 파티클을 저장하는 풀

    public static BulletManager instance = null;                                    // 싱글턴 패턴을 위한 인스턴스

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // 기본 총알을 미리 생성해 풀에 저장
        for(int i = 0; i < maxNormalBullet; i++)
        {
            GameObject playerBullet = Instantiate(normalBulletPrefab);
            playerBullet.name = "NormalBullet_" + i.ToString();
            playerBullet.SetActive(false);
            normalBulletPool.Add(playerBullet);
        }

        // 머신건 총알을 미리 생성해 풀에 저장
        for (int i = 0; i < maxMachineGunBullet; i++)
        {
            GameObject playerBullet = Instantiate(machineGunBulletPrefab);
            playerBullet.name = "MachineGunBullet_" + i.ToString();
            playerBullet.SetActive(false);
            machineGunBulletPool.Add(playerBullet);
        }

        // 유탄을 미리 생성해 풀에 저장
        for (int i = 0; i < maxGrenade; i++)
        {
            GameObject playerBullet = Instantiate(grenadePrefab);
            playerBullet.name = "Grenade_" + i.ToString();
            playerBullet.SetActive(false);
            grenadePool.Add(playerBullet);
        }

        // 적이 날리는 단검을 미리 생성해 풀에 저장
        for (int i = 0; i < maxDagger; i++)
        {
            GameObject dagger = Instantiate(daggerPrefab);
            dagger.name = "ThrowingDagger_" + i.ToString();
            dagger.SetActive(false);
            daggerPool.Add(dagger);
        }

        // 피탄 파티클을 미리 생성해 풀에 저장
        for (int i = 0; i < maxFlare; i++)
        {
            GameObject flare = Instantiate(flarePrefab);
            flare.name = "Flare_" + i.ToString();
            flare.SetActive(false);
            flarePool.Add(flare);
        }
    }
}
