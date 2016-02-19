using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 게임에 사용되는 아이템을 정해진 수 만큼 미리 저장
// 게임 레벨마다 사용되는 아이템 수도 지정

public class ItemManager : MonoBehaviour {

    // 게임 레벨마다 생성될 아이템 수를 담는 클래스
    public class LevelItemNum
    {
        public int firstAid;
        public int ammoBox;
        public int itemGrenade;

        public LevelItemNum(int firstAid, int ammoBox, int itemGrenade)
        {
            this.firstAid = firstAid;
            this.ammoBox = ammoBox;
            this.itemGrenade = itemGrenade;
        }
    }

    public int maxFirstAid = 10;
    public int maxAmmoBox = 10;
    public int maxItemGrenade = 10;

    public int level1FirstAid;
    public int level1AmmoBox;
    public int level1ItemGrenade;

    public int level2FirstAid;
    public int level2AmmoBox;
    public int level2ItemGrenade;

    public int level3FirstAid;
    public int level3AmmoBox;
    public int level3ItemGrenade;

    public int level4FirstAid;
    public int level4AmmoBox;
    public int level4ItemGrenade;

    public int level5FirstAid;
    public int level5AmmoBox;
    public int level5ItemGrenade;

    public int level6FirstAid;
    public int level6AmmoBox;
    public int level6ItemGrenade;

    public GameObject firstAidPrefab;
    public GameObject ammoBoxPrefab;
    public GameObject itemGrenadePrefab;

    public List<GameObject> firstAidPool = new List<GameObject>();                  // 구급 상자를 미리 저장하는 풀
    public List<GameObject> ammoBoxPool = new List<GameObject>();                   // 탄약 상자를 미리 저장하는 풀
    public List<GameObject> itemGrenadePool = new List<GameObject>();               // 유탄 아이템을 미리 저장하는 풀

    public List<LevelItemNum> levelItemNumList = new List<LevelItemNum>();          // LevelItemNum을 순서대로 참조하기 위해 리스트로 관리

    public static ItemManager instance = null;                                      // 싱글턴 패턴을 위한 인스턴스

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // 구급 상자를 미리 생성해 풀에 저장
        for (int i = 0; i < maxFirstAid; i++)
        {
            GameObject firstAid = Instantiate(firstAidPrefab);
            firstAid.name = "FirstAid_" + i.ToString();
            firstAid.SetActive(false);
            firstAidPool.Add(firstAid);
        }

        // 탄약 상자를 미리 생성해 풀에 저장
        for (int i = 0; i < maxAmmoBox; i++)
        {
            GameObject ammoBox = Instantiate(ammoBoxPrefab);
            ammoBox.name = "AmmoBox_" + i.ToString();
            ammoBox.SetActive(false);
            ammoBoxPool.Add(ammoBox);
        }

        // 유탄 아이템을 미리 생성해 풀에 저장
        for (int i = 0; i < maxItemGrenade; i++)
        {
            GameObject itemGrenade = Instantiate(itemGrenadePrefab);
            itemGrenade.name = "ItemGrenade_" + i.ToString();
            itemGrenade.SetActive(false);
            itemGrenadePool.Add(itemGrenade);
        }

        SetLevelItemNumList();
    }

    void SetLevelItemNumList()
    {
        LevelItemNum level1ItemNum = new LevelItemNum(level1FirstAid, level1AmmoBox, level1ItemGrenade);
        LevelItemNum level2ItemNum = new LevelItemNum(level2FirstAid, level2AmmoBox, level2ItemGrenade);
        LevelItemNum level3ItemNum = new LevelItemNum(level3FirstAid, level3AmmoBox, level3ItemGrenade);
        LevelItemNum level4ItemNum = new LevelItemNum(level4FirstAid, level4AmmoBox, level4ItemGrenade);
        LevelItemNum level5ItemNum = new LevelItemNum(level5FirstAid, level5AmmoBox, level5ItemGrenade);
        LevelItemNum level6ItemNum = new LevelItemNum(level6FirstAid, level6AmmoBox, level6ItemGrenade);

        levelItemNumList.Add(level1ItemNum);
        levelItemNumList.Add(level2ItemNum);
        levelItemNumList.Add(level3ItemNum);
        levelItemNumList.Add(level4ItemNum);
        levelItemNumList.Add(level5ItemNum);
        levelItemNumList.Add(level6ItemNum);
    }
}
