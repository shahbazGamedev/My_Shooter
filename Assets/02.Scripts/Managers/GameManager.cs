using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    // 레벨마다 생성될 적의 수를 담는 클래스
    public class LevelEnemyNum
    {
        public int normal;
        public int spear;
        public int mage;

        public LevelEnemyNum(int normal, int spear, int mage)
        {
            this.normal = normal;
            this.spear = spear;
            this.mage = mage;
        }
    }

    public int gameLevel = 1;                               // 현재 게임의 레벨
    public int maxGameLevel = 6;                            // 최대 게임 레벨
    public float enemySpawnDelay;                           // 적 하나가 생성되는 딜레이
    public float itemSpawnDelay;
    public bool isGameOver = false;                         // 플레이어 사망으로 게임 종료 시 true
    public bool isGameClear = false;                        // 게임 조건 만족으로 게임 클리어 시 true
    public bool isGameTimeUp = false;                       // 게임 종료 시간까지 버티면 true

    public int enemyCount = 0;                              // 현재 생성된 적의 수
    public int enemyHpUp;                                   // 게임 레벨 3, 5에서 이 수치만큼 적의 체력이 증가

    public static GameManager instance = null;              // 싱글턴 패턴을 위한 인스턴스

    // 게임 레벨에 따라 Level 1 몬스터부터 6까지 누적되서 enemyList에 들어간다.
    public int level1NumNormal;                     // 레벨1의 NormalSkeleton의 수
    public int level1NumSpear;                      // 레벨1의 SpearSkeleton의 수
    public int level1NumMage;                       // 레벨1의 MageSkeleton의 수

    public int level2NumNormal;
    public int level2NumSpear;
    public int level2NumMage;

    public int level3NumNormal;
    public int level3NumSpear;
    public int level3NumMage;

    public int level4NumNormal;
    public int level4NumSpear;
    public int level4NumMage;

    public int level5NumNormal;
    public int level5NumSpear;
    public int level5NumMage;

    public int level6NumNormal;
    public int level6NumSpear;
    public int level6NumMage;

    public GameObject normalSkelPrefab;
    public GameObject spearSkelPrefab;
    public GameObject mageSkelPrefab;
    public GameObject helicopter;
    public GameObject gameClearColl;
    public Light mapLight;
    public Text timeText;
    public Text alertText;

    public Transform[] enemySpawnPoints;
    public Transform[] itemSpawnPoints;

    public List<LevelEnemyNum> levelEnemyNumList = new List<LevelEnemyNum>(); // LevelEnemyNum을 순서대로 참조하기 위해 리스트로 관리
    public List<GameObject> enemyList = new List<GameObject>();               // SpawnEnemy에서 참조할 현재 생성된 모든 적의 리스트

    private float gameTimer = 0f;                                             // 게임 내 시간
    private float defaultLightIntensity;                                      // 초기 빛의 세기를 이 변수에 저장해둠

    const float eachLevelTime = 60f;                                          // 한 레벨의 시간. 지정된 시간이 지나면 게임 레벨이 오른다.
    const float eachLevelLight = 0.15f;                                       // 게임 레벨이 오를 때 마다 이 수치만큼 빛이 약해진다.

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        enemySpawnPoints = GameObject.Find("EnemySpawnPoints").GetComponentsInChildren<Transform>();
        itemSpawnPoints = GameObject.Find("ItemSpawnPoints").GetComponentsInChildren<Transform>();
        defaultLightIntensity = mapLight.intensity;

        SetLevelEnemyNumList();
        SetLightIntensity();
        SetTimeText();
        alertText.text = "";

        // 초기 설정된 레벨만큼 적을 생성
        for (int i = 0; i < gameLevel; i++)
        {
            CreateEnemy(levelEnemyNumList[i], i + 1);
        }

        StartCoroutine(SpawnEnemy());
        StartCoroutine(SpawnItem());
    }

    void Update()
    {
        if (isGameOver)
            return;

        gameTimer += Time.deltaTime;

        // 정해진 시간이 지나면 게임 레벨이 증가하고 그만큼 더 많은 적이 생성
        if(gameTimer >= eachLevelTime)
        {
            GameLevelUp();
        }
    }

    void OnEnable()
    {
        PlayerHealth.OnPlayerDie += this.OnPlayerDie;   // 플레이어 사망, 게임 클리어 이벤트 함수 추가
        PlayerGameClear.OnGameClear += this.OnGameClear;
    }

    void OnDisable()
    {
        PlayerHealth.OnPlayerDie -= this.OnPlayerDie;
        PlayerGameClear.OnGameClear -= this.OnGameClear;
    }

    // 함수 : SetLevelEnemyNumList
    // 목적 : 입력된 적의 수를 기반으로 levelEnemyNumList 초기화
    void SetLevelEnemyNumList()
    {
        LevelEnemyNum level1EnemyNum = new LevelEnemyNum(level1NumNormal, level1NumSpear, level1NumMage);
        LevelEnemyNum level2EnemyNum = new LevelEnemyNum(level2NumNormal, level2NumSpear, level2NumMage);
        LevelEnemyNum level3EnemyNum = new LevelEnemyNum(level3NumNormal, level3NumSpear, level3NumMage);
        LevelEnemyNum level4EnemyNum = new LevelEnemyNum(level4NumNormal, level4NumSpear, level4NumMage);
        LevelEnemyNum level5EnemyNum = new LevelEnemyNum(level5NumNormal, level5NumSpear, level5NumMage);
        LevelEnemyNum level6EnemyNum = new LevelEnemyNum(level6NumNormal, level6NumSpear, level6NumMage);

        levelEnemyNumList.Add(level1EnemyNum);
        levelEnemyNumList.Add(level2EnemyNum);
        levelEnemyNumList.Add(level3EnemyNum);
        levelEnemyNumList.Add(level4EnemyNum);
        levelEnemyNumList.Add(level5EnemyNum);
        levelEnemyNumList.Add(level6EnemyNum);
    }

    // 함수 : CreateEnemy
    // 목적 : 레벨마다 설정된 적의 수 만큼 생성하고 enemyList에 추가
    void CreateEnemy(LevelEnemyNum num, int level)
    {
        // NormalSkeleton 생성
        for(int i = 0; i < num.normal; i++)
        {
            GameObject enemy = Instantiate(normalSkelPrefab);
            enemy.name = "Level" + level.ToString() + "NormalSkeleton_" + i.ToString();
            enemy.SetActive(false);
            enemyList.Add(enemy);
        }

        // SpearSkeleton 생성
        for (int i = 0; i < num.spear; i++)
        {
            GameObject enemy = Instantiate(spearSkelPrefab);
            enemy.name = "Level" + level.ToString() + "SpearSkeleton_" + i.ToString();
            enemy.SetActive(false);
            enemyList.Add(enemy);
        }

        // MageSkeleton 생성
        for (int i = 0; i < num.mage; i++)
        {
            GameObject enemy = Instantiate(mageSkelPrefab);
            enemy.name = "Level" + level.ToString() + "MageSkeleton_" + i.ToString();
            enemy.SetActive(false);
            enemyList.Add(enemy);
        }
    }

    // 함수 : SpawnEnemy
    // 목적 : enemyList에 등록된 비활성화된 적을 활성화 시키고 랜덤한 스폰 지역에 소환
    IEnumerator SpawnEnemy()
    {
        while(!isGameOver)
        {
            yield return new WaitForSeconds(enemySpawnDelay);

            // 플레이어 사망시 코루틴 종료
            if (isGameOver) yield break;

            // 모든 적이 활성화 되어 있으면 작업할 필요 없음
            if (enemyCount >= enemyList.Count)
                continue;

            int enemyIdx = Random.Range(0, enemyList.Count - 1);     // 리스트 중에서 랜덤한 적 선택
            while(enemyList[enemyIdx].activeSelf)                    // 그 적이 이미 활성화 되어 있다면 비활성화 된 적을 찾을때까지 루프
            {
                enemyIdx++;
                if (enemyIdx >= enemyList.Count)
                    enemyIdx = 0;
            }

            int spawnPointIdx = Random.Range(1, enemySpawnPoints.Length - 1);                   // 랜덤한 스폰 위치를 선택

            // 선택된 스폰 위치로 자리를 옮기고 활성화
            enemyList[enemyIdx].transform.position = enemySpawnPoints[spawnPointIdx].position;
            enemyList[enemyIdx].transform.rotation = enemySpawnPoints[spawnPointIdx].rotation;
            enemyList[enemyIdx].SetActive(true);

            enemyCount++; // 적 카운트 증가
        }
    }

    // 함수 : GameLevelUp
    // 목적 : 게임 레벨을 증가시키고 해당 게임 레벨에 맞는 적을 생성
    void GameLevelUp()
    {
        gameTimer = 0f; // 게임 타이머 초기화
        
        if (gameLevel < maxGameLevel) // 게임 레벨이 아직 최대치가 아닐 때
        {
            gameLevel++; // 게임 레벨 증가

            // 게임 레벨이 3 또는 5라면 메시지 출력
            if (gameLevel == 3 || gameLevel == 5)
            {
                StartCoroutine(DisplayAlertText("적의 체력이 높아집니다"));
            }

            CreateEnemy(levelEnemyNumList[gameLevel - 1], gameLevel);   // 게임 레벨에 맞는 적 생성
            SetLightIntensity();                                        // 게임 레벨에 따라 빛의 세기 조절해 어두워짐
            SetTimeText();                                              // 시간 텍스트 변경
            StartCoroutine(SpawnItem());                                // 게임 레벨에 맞는 아이템 생성
        }
        else if(gameLevel >= maxGameLevel)  // 게임 최고 레벨이면서 시간이 다 됐다
        {
            GameTimeUp();
        }
    }

    // 함수 : SetLightIntensity
    // 목적 : 현재 게임 레벨에 따른 조명을 설정. 게임 레벨이 오를수록 어두워짐
    void SetLightIntensity()
    {
        mapLight.intensity = defaultLightIntensity - eachLevelLight * (gameLevel - 1);
    }

    // 함수 : SetTimeText
    // 목적 : 게임 시간 표시 UI를 세팅
    void SetTimeText()
    {
        int time = 18;
        time += gameLevel - 1;

        timeText.text = time + ":00";
    }

    // 함수 : SpawnItem
    // 목적 : 현재 게임 레벨에 할당된 보급 아이템을 맵에 생성
    //        게임 시작할 때 Start에서, 게임 레벨이 오를 때 GameLevelUp에서 호출
    IEnumerator SpawnItem()
    {
        // 이전에 사용해서 비활성화된 아이템 스폰 지점을 다시 활성화시킴
        for(int i = 1; i < itemSpawnPoints.Length - 1; i++)
        {
            if (!itemSpawnPoints[i].gameObject.activeSelf)
                itemSpawnPoints[i].gameObject.SetActive(true);
        }

        // itemSpawnDelay 만큼 지연됐다가 아이템 생성
        yield return new WaitForSeconds(itemSpawnDelay);

        if (isGameOver)
            yield break;

        // 게임 레벨에 따라 설정된 구급 상자 갯수만큼 맵에 생성
        for(int i = 0; i <ItemManager.instance.levelItemNumList[gameLevel - 1].firstAid; i++)
        {
            int spawnPointIdx = GetItemSpawnIdx();

            foreach (GameObject firstAid in ItemManager.instance.firstAidPool)
            {
                // 활성화 되지 않은 구급 상자를 활성화시켜 스폰 지역으로 이동
                if (!firstAid.activeSelf)
                {
                    firstAid.transform.position = itemSpawnPoints[spawnPointIdx].transform.position;
                    firstAid.SetActive(true);
                    itemSpawnPoints[spawnPointIdx].gameObject.SetActive(false);
                    break;
                }
            }
        }

        // 게임 레벨에 따라 설정된 탄약 상자 갯수만큼 맵에 생성
        for (int i = 0; i < ItemManager.instance.levelItemNumList[gameLevel - 1].ammoBox; i++)
        {
            int spawnPointIdx = GetItemSpawnIdx();

            foreach (GameObject ammoBox in ItemManager.instance.ammoBoxPool)
            {
                if (!ammoBox.activeSelf)
                {
                    ammoBox.transform.position = itemSpawnPoints[spawnPointIdx].transform.position;
                    ammoBox.SetActive(true);
                    itemSpawnPoints[spawnPointIdx].gameObject.SetActive(false);
                    break;
                }
            }
        }

        // 게임 레벨에 따라 설정된 유탄 아이템 갯수만큼 맵에 생성
        for (int i = 0; i < ItemManager.instance.levelItemNumList[gameLevel - 1].itemGrenade; i++)
        {
            int spawnPointIdx = GetItemSpawnIdx();

            foreach (GameObject itemGrenade in ItemManager.instance.itemGrenadePool)
            {
                if (!itemGrenade.activeSelf)
                {
                    itemGrenade.transform.position = itemSpawnPoints[spawnPointIdx].transform.position;
                    itemGrenade.SetActive(true);
                    itemSpawnPoints[spawnPointIdx].gameObject.SetActive(false);
                    break;
                }
            }
        }

        StartCoroutine(DisplayAlertText("아이템이 생성되었습니다"));
    }

    // 함수 : GetItemSpawnIdx
    // 목적 : 아이템 스폰 지점 중 스폰 가능한 곳(오브젝트가 활성화 된 곳)의 번호를 반환
    //        아이템이 같은 위치에 중복 소환되는 것을 막기 위해서
    int GetItemSpawnIdx()
    {
        int itemSpawnIdx = Random.Range(1, itemSpawnPoints.Length - 1);

        for (int i = 0; i < itemSpawnPoints.Length - 2; i++)
        {
            // 아이템 스폰 지점 중 활성화 된 곳의 인덱스를 반환
            // 비활성화 되어 있다면 이미 아이템이 생성된 지점이라는 뜻
            if (itemSpawnPoints[itemSpawnIdx].gameObject.activeSelf)
            {
                return itemSpawnIdx;
            }
            else
            {
                itemSpawnIdx++;

                if (itemSpawnIdx >= itemSpawnPoints.Length)
                    itemSpawnIdx = 1;
            }
        }

        return itemSpawnIdx;
    }

    // 함수 : DisplayAlertText
    // 목적 : UI의 AlertText에 메시지 표시하고 지움
    IEnumerator DisplayAlertText(string text)
    {
        alertText.text = text;

        yield return new WaitForSeconds(3f);

        alertText.text = "";
    }

    // 함수 : OnPlayerDie
    // 목적 : 플레이어 사망 이벤트. 게임 오버 처리
    void OnPlayerDie()
    {
        isGameOver = true;
        StopAllCoroutines();
    }

    // 함수 : OnGameClear
    // 목적 : 게임 클리어 이벤트 함수
    void OnGameClear()
    {
        isGameClear = true;
        StopAllCoroutines();
    }

    // 함수 : GameTimeUp
    // 목적 : 0시까지 버티는데 성공하면 호출
    //        헬기와 게임 클리어 컬라이더를 활성화
    void GameTimeUp()
    {
        isGameTimeUp = true;
        timeText.text = "0:00";

        helicopter.SetActive(true);
        gameClearColl.SetActive(true);

        StartCoroutine(DisplayAlertText("7시 방향 헬기장으로 탈출하세요!"));
    }
}
