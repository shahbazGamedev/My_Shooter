using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 플레이어의 총 발사를 처리하는 스크립트

public class PlayerShooting : MonoBehaviour {

    // 플레이어의 무기. 열거형
    public enum CurrentWeapon
    {
        normalGun,
        machineGun,
        grenadeLauncher
    };

    public CurrentWeapon curWeapon = CurrentWeapon.normalGun;   // 사용중인 무기

    public float shootDelay;                                    // 현재 상태의 공격 딜레이
    public float normalGunDelay = 0.13f;                        // 기본 총의 공격 딜레이
    public float machineGunDelay = 0.05f;                       // 머신건의 공격 딜레이
    public float grenadeLauncherDelay = 1.0f;                   // 유탄 발사기의 공격 딜레이
    public int machineGunAmmo = 0;                              // 머신건의 남은 탄환
    public int grenadeLauncherAmmo = 0;                         // 유탄 발사기의 남은 탄환

    public Text weaponText;                                     // 무기 이름 텍스트
    public Text ammoText;                                       // 남은 탄 텍스트

    public Image normalBtnImg;                                  // 기본 총 버튼
    public Image machineBtnImg;                                 // 머신건 버튼
    public Image grenadeBtnImg;                                 // 유탄 발사기 버튼

    private float timer = 0f;                                   // 공격 딜레이 처리를 위한 타이머
    private float effectsDisplayTime = 0.2f;                    // 무기 발사 파티클의 딜레이

    private Light gunLight;                                     // 무기 발사 조명
    private ParticleSystem gunParticle;                         // 무기 발사 파티클

    private Color clearColor = new Color(1f, 1f, 1f, 1f);           // 완전 불투명 컬러
    private Color transparentColor = new Color(1f, 1f, 1f, 0.5f);   // 반투명 컬러

    void Awake()
    {
        gunParticle = GetComponent<ParticleSystem>();
        gunLight = GetComponent<Light>();

        shootDelay = normalGunDelay;

        DisplayAmmo();
        SetBtnImgAlpha();
    }

    void Update()
    {
        timer += Time.deltaTime;

#if UNITY_EDITOR
        // 에디터 전용 총 발사, 무기 교체
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.green);
        ClickFirePC(ray);
        SwapWeaponPC(ray);
#endif

        TouchInput(); // 안드로이드 터치 입력

        // 총 발사로 생기는 빛 비활성화
        if (timer >= shootDelay * effectsDisplayTime)
        {
            DisableEffect();
        }
    }

    // 함수 : ClickFirePC
    // 목적 : 무기 발사 버튼을 클릭 했는지 Raycast로 확인하고 발사
    //        PC 버전 전용
    void ClickFirePC(Ray ray)
    {
        RaycastHit hit;

        if (Input.GetMouseButton(0) && timer >= shootDelay)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1<<LayerMask.NameToLayer("VRPad")))
            {
                if (hit.collider.CompareTag("BUTTON_FIRE"))
                {
                    Shoot();
                }
            }
        }
    }

    // 함수 : SwapWeaponPC
    // 목적 : 선택한 버튼에 따라 현재 무기와 공격 딜레이를 변경
    //        PC 버전 전용
    void SwapWeaponPC(Ray ray)
    {
        RaycastHit hit;

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("VRPad")))
            {
                if (hit.collider.CompareTag("BUTTON_NORMALGUN"))
                {
                    curWeapon = CurrentWeapon.normalGun;
                    shootDelay = normalGunDelay;
                    weaponText.text = "Normal Gun";
                }
                else if (hit.collider.CompareTag("BUTTON_MACHINEGUN"))
                {
                    curWeapon = CurrentWeapon.machineGun;
                    shootDelay = machineGunDelay;
                    weaponText.text = "Machine Gun";
                }
                else if (hit.collider.CompareTag("BUTTON_GRENADE"))
                {
                    curWeapon = CurrentWeapon.grenadeLauncher;
                    shootDelay = grenadeLauncherDelay;
                    weaponText.text = "Grenade Launcher";
                }
            }

            // 무기가 바뀌었으므로 그에 맞는 텍스트와 이미지 변경
            DisplayAmmo();
            SetBtnImgAlpha();
        }
#endif
    }

    // 함수 : SwapWeapon
    // 목적 : hit 된 컬라이더의 태그에 따라 무기를 교체
    //        TouchInput에서 호출
    void SwapWeapon(RaycastHit hit)
    {
        if (hit.collider.CompareTag("BUTTON_NORMALGUN"))
        {
            curWeapon = CurrentWeapon.normalGun;
            shootDelay = normalGunDelay;
            weaponText.text = "Normal Gun";
        }
        else if (hit.collider.CompareTag("BUTTON_MACHINEGUN"))
        {
            curWeapon = CurrentWeapon.machineGun;
            shootDelay = machineGunDelay;
            weaponText.text = "Machine Gun";
        }
        else if (hit.collider.CompareTag("BUTTON_GRENADE"))
        {
            curWeapon = CurrentWeapon.grenadeLauncher;
            shootDelay = grenadeLauncherDelay;
            weaponText.text = "Grenade Launcher";
        }

        // 무기가 바뀌었으므로 그에 맞는 텍스트와 이미지 변경
        DisplayAmmo();
        SetBtnImgAlpha();
    }

    // 함수 : TouchInput
    // 목적 : 터치 입력 처리. 공격 버튼과 무기 교체 버튼에 대해서 처리한다
    //        Update에서 호출
    void TouchInput()
    {
        if (Input.touchCount > 0)
        {
            RaycastHit hit;

            for (int i = 0; i < Input.touchCount; i++)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);

                // 발사 버튼은 계속 누르고 있는 상태일 때 처리. 연타할 필요 없이 누르고 있으면 연속 발사한다.
                if (Input.GetTouch(i).phase == TouchPhase.Moved || Input.GetTouch(i).phase == TouchPhase.Stationary)
                {
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("VRPad")))
                    {
                        if (hit.collider.CompareTag("BUTTON_FIRE") && timer >= shootDelay)
                            Shoot();
                    }
                }

                // 무기 교체는 한번 눌렀을 때 처리
                if(Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("VRPad")))
                    {
                        SwapWeapon(hit);
                    }
                }
            }
        }
    }

    // 함수 : DisplayAmmo
    // 목적 : 지금 사용하는 무기와 남은 탄에 맞게 텍스트를 표시
    //        탄환이 변하는 작업을 하는 함수에서 호출
    void DisplayAmmo()
    {
        switch (curWeapon)
        {
            case CurrentWeapon.normalGun:
                ammoText.text = "Ammo ∞";
                break;

            case CurrentWeapon.machineGun:
                ammoText.text = "Ammo " + machineGunAmmo;
                break;

            case CurrentWeapon.grenadeLauncher:
                ammoText.text = "Ammo " + grenadeLauncherAmmo;
                break;
        }
    }

    // 함수 : DisableEffect
    // 목적 : 무기 발사 시 생긴 빛을 비활성화
    void DisableEffect()
    {
        gunLight.enabled = false;
    }

    // 함수 : Shoot
    // 목적 : 지금 가진 무기에 맞는 총알을 발사
    void Shoot()
    {
        switch(curWeapon)
        {
            case CurrentWeapon.normalGun:
                CreateNormalBullet();
                break;

            case CurrentWeapon.machineGun:
                // 남은 총알이 없으면 발사하지 못함
                if (machineGunAmmo <= 0)
                    return;
                CreateMachineGunBullet();
                break;

            case CurrentWeapon.grenadeLauncher:
                if (grenadeLauncherAmmo <= 0)
                    return;
                CreateGrenade();
                break;
        }

        DisplayAmmo();

        gunLight.enabled = true;

        gunParticle.Stop();
        gunParticle.Play();
        timer = 0.0f;
    }

    // 함수 : CreateNormalBullet
    // 목적 : 오브젝트 풀에 있는 일반 총알 생성
    void CreateNormalBullet()
    {
        foreach(GameObject bullet in BulletManager.instance.normalBulletPool)
        {
            // 활성화 되지 않은 총알을 활성화시켜 발사
            if(!bullet.activeSelf)
            {
                bullet.transform.position = transform.position;
                bullet.transform.rotation = transform.rotation;
                bullet.SetActive(true);
                break;
            }
        }
    }

    // 함수 : CreateMachineGunBullet
    // 목적 : 오브젝트 풀에 있는 머신건 총알 생성
    void CreateMachineGunBullet()
    {
        foreach (GameObject bullet in BulletManager.instance.machineGunBulletPool)
        {
            // 활성화 되지 않은 총알을 활성화시켜 발사
            if (!bullet.activeSelf)
            {
                bullet.transform.position = transform.position;
                bullet.transform.rotation = transform.rotation;
                bullet.SetActive(true);
                break;
            }
        }

        machineGunAmmo--;
    }

    // 함수 : CreateGrenade
    // 목적 : 오브젝트 풀에 있는 유탄 생성
    void CreateGrenade()
    {
        foreach (GameObject bullet in BulletManager.instance.grenadePool)
        {
            // 활성화 되지 않은 유탄을 활성화시켜 발사
            if (!bullet.activeSelf)
            {
                bullet.transform.position = transform.position;
                bullet.transform.rotation = transform.rotation;
                bullet.SetActive(true);
                break;
            }
        }

        grenadeLauncherAmmo--;
    }

    // 함수 : GetAmmoBox
    // 목적 : 머신건 탄을 획득. PlayerHealth의 아이템 획득 부분에서 호출
    public void GetAmmoBox(int ammo)
    {
        machineGunAmmo += ammo;
        DisplayAmmo();
    }

    // 함수 : GetGrenade
    // 목적 : 유탄을 획득. PlayerHealth의 아이템 획득 부분에서 호출
    public void GetGrenade(int ammo)
    {
        grenadeLauncherAmmo += ammo;
        DisplayAmmo();
    }

    // 함수 : SetBtnImgAlpha
    // 목적 : 사용중인 무기의 버튼을 선명하게, 아닌 무기는 반투명하게 설정
    void SetBtnImgAlpha()
    {
        switch (curWeapon)
        {
            case CurrentWeapon.normalGun:
                normalBtnImg.color = clearColor;
                machineBtnImg.color = transparentColor;
                grenadeBtnImg.color = transparentColor;
                break;

            case CurrentWeapon.machineGun:
                normalBtnImg.color = transparentColor;
                machineBtnImg.color = clearColor;
                grenadeBtnImg.color = transparentColor;
                break;

            case CurrentWeapon.grenadeLauncher:
                normalBtnImg.color = transparentColor;
                machineBtnImg.color = transparentColor;
                grenadeBtnImg.color = clearColor;
                break;
        }
    }
}
