using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

    public int maxHp = 100;
    public int currentHp;
    public bool isDead = false;

    public GameObject bloodEffect;
    public Slider hpSlider;
    public Image hpFillImage;
    public Text hpText;

    public static PlayerHealth instance = null;                     // 싱글턴 패턴을 위한 인스턴스

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

    void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHp -= damage;

        CreateBloodEffect(transform.position + transform.up);

        if (currentHp <= 0)
        {
            currentHp = 0;
            PlayerDie();
        }
        SetHealthUI();
    }

    void PlayerDie()
    {
        anim.SetTrigger("Die");

        isDead = true;
        GameManager.instance.isGameOver = true;

        OnPlayerDie();                               // 사망 이벤트 발생

        playerMovement.enabled = false;
        playerShooting.enabled = false;
    }

    void CreateBloodEffect(Vector3 pos)
    {
        GameObject blood = (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity);
        Destroy(blood, 1.0f);
    }

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

    void GetFirstAid(int healPoint)
    {
        currentHp += healPoint;

        if (currentHp > maxHp)
            currentHp = maxHp;

        SetHealthUI();
    }
}
