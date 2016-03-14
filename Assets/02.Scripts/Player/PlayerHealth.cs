using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 플레이어 체력 및 아이템 획득 관련 스크립트

public class PlayerHealth : MonoBehaviour {

    public int maxHp = 100;                 // 최대 체력
    public int currentHp;                   // 현재 체력
    public bool isDead = false;             // 사망 여부

    public GameObject bloodEffect;
    public Slider hpSlider;
    public Image hpFillImage;
    public Text hpText;

    public static PlayerHealth instance = null;     // 싱글턴 패턴을 위한 인스턴스

    // 델리게이트와 이벤트 선언. 플레이어 사망시 호출
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;

    private Animator anim;
    private PlayerMovement playerMovement;
    private PlayerShooting playerShooting;

    void Awake()
    {
        instance = this;

        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooting = GetComponentInChildren<PlayerShooting>();

        currentHp = maxHp;
        hpSlider.maxValue = maxHp;
        SetHealthUI();
    }

    void OnTriggerEnter(Collider coll)
    {
        // 피격과 아이템 획득을 같이 처리
        if (coll.CompareTag("ENEMY_WEAPON"))
        {
            TakeDamage(coll.GetComponent<SkeletonWeapon>().damage);
        }
        else if (coll.CompareTag("ENEMY_BULLET"))
        {
            TakeDamage(coll.GetComponent<BulletCtrl>().damage);
            coll.gameObject.SetActive(false);
        }
        else if (coll.CompareTag("ITEM_FIRST_AID"))
        {
            GetFirstAid(coll.GetComponent<FirstAid>().healPoint);
            coll.gameObject.SetActive(false);
        }
        else if (coll.CompareTag("ITEM_AMMO_BOX"))
        {
            playerShooting.GetAmmoBox(coll.GetComponent<AmmoBox>().ammo);
            coll.gameObject.SetActive(false);
        }
        else if(coll.CompareTag("ITEM_GRENADE"))
        {
            playerShooting.GetGrenade(coll.GetComponent<ItemGrenade>().ammo);
            coll.gameObject.SetActive(false);
        }
    }

    // 함수 : TakeDamage
    // 목적 : 피격 처리
    void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHp -= damage;

        CreateBloodEffect(transform.position + transform.up); // 피격 시 발생하는 선혈 효과

        // HP가 다 되면 사망
        if (currentHp <= 0)
        {
            currentHp = 0;
            PlayerDie();
        }

        SetHealthUI();
    }

    // 함수 : PlayerDie
    // 목적 : 플레이어 사망 처리
    void PlayerDie()
    {
        anim.SetTrigger("Die"); // 사망 애니메이션 실행

        isDead = true;
        GameManager.instance.isGameOver = true;

        OnPlayerDie(); // 사망 이벤트 발생

        playerMovement.enabled = false;
        playerShooting.enabled = false;
    }

    // 함수 : CreateBloodEffect
    // 목적 : 피격 시 선혈 효과를 발생시킴. TakeDamage에서 호출
    void CreateBloodEffect(Vector3 pos)
    {
        GameObject blood = (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity);
        Destroy(blood, 1.0f);
    }

    // 함수 : SetHealthUI
    // 목적 : 현재 체력에 맞는 HP UI를 세팅
    void SetHealthUI()
    {
        hpSlider.value = currentHp;
        hpText.text = currentHp.ToString();

        // 체력 60% 이상 녹색
        //      30% 이상 노란색
        //          이하 빨간색
        if ( (float)currentHp / maxHp >= 0.6f)
            hpFillImage.color = Color.green;
        else if ( (float)currentHp / maxHp >= 0.3f)
            hpFillImage.color = Color.yellow;
        else
            hpFillImage.color = Color.red;
    }

    // 함수 : GetFirstAid
    // 목적 : 구급 상자 아이템을 얻어 체력 회복
    void GetFirstAid(int healPoint)
    {
        currentHp += healPoint;

        // 최대 체력보다 높게 회복되진 않는다
        if (currentHp > maxHp)
            currentHp = maxHp;

        SetHealthUI();
    }
}
