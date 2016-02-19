using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerShooting : MonoBehaviour {

    public enum CurrentWeapon
    {
        normalGun,
        machineGun,
        grenadeLauncher
    };

    public CurrentWeapon curWeapon = CurrentWeapon.normalGun;

    public float shootDelay;                                    // 현재 상태의 공격 딜레이
    public float normalGunDelay = 0.13f;                        // 기본 총의 공격 딜레이
    public float machineGunDelay = 0.05f;                       // 머신건의 공격 딜레이
    public float grenadeLauncherDelay = 1.0f;                   // 유탄 발사기의 공격 딜레이
    public int machineGunAmmo = 0;                              // 머신건의 남은 탄환
    public int grenadeLauncherAmmo = 0;                         // 유탄 발사기의 남은 탄환

    public Text WeaponText;
    public Text AmmoText;

    private float timer = 0f;
    private float effectsDisplayTime = 0.2f;

    private Light gunLight;
    private ParticleSystem gunParticle;

    void Awake()
    {
        gunParticle = GetComponent<ParticleSystem>();
        gunLight = GetComponent<Light>();

        shootDelay = normalGunDelay;

        DisplayAmmo();
    }

    void Update()
    {
        timer += Time.deltaTime;

#if UNITY_EDITOR
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.green);
        ClickFirePC(ray);
        SwapWeaponPC(ray);
#endif

        TouchInput();

        if (timer >= shootDelay * effectsDisplayTime)
        {
            DisableEffect();
        }
    }

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

    // 선택한 버튼에 따라 현재 무기와 공격 딜레이를 변경
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
                    WeaponText.text = "Normal Gun";
                }
                else if (hit.collider.CompareTag("BUTTON_MACHINEGUN"))
                {
                    curWeapon = CurrentWeapon.machineGun;
                    shootDelay = machineGunDelay;
                    WeaponText.text = "Machine Gun";
                }
                else if (hit.collider.CompareTag("BUTTON_GRENADE"))
                {
                    curWeapon = CurrentWeapon.grenadeLauncher;
                    shootDelay = grenadeLauncherDelay;
                    WeaponText.text = "Grenade Launcher";
                }
            }

            DisplayAmmo();
        }
#endif
    }

    void SwapWeapon(RaycastHit hit)
    {
        if (hit.collider.CompareTag("BUTTON_NORMALGUN"))
        {
            curWeapon = CurrentWeapon.normalGun;
            shootDelay = normalGunDelay;
            WeaponText.text = "Normal Gun";
        }
        else if (hit.collider.CompareTag("BUTTON_MACHINEGUN"))
        {
            curWeapon = CurrentWeapon.machineGun;
            shootDelay = machineGunDelay;
            WeaponText.text = "Machine Gun";
        }
        else if (hit.collider.CompareTag("BUTTON_GRENADE"))
        {
            curWeapon = CurrentWeapon.grenadeLauncher;
            shootDelay = grenadeLauncherDelay;
            WeaponText.text = "Grenade Launcher";
        }

        DisplayAmmo();
    }

    // 터치 입력 처리. 공격 버튼과 무기 교체 버튼에 대해서만 처리한다
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

    // 지금 가진 무기와 탄환에 맞게 잔탄 텍스트를 표시
    // 탄환이 변하는 작업을 하는 함수에서 호출
    void DisplayAmmo()
    {
        switch (curWeapon)
        {
            case CurrentWeapon.normalGun:
                AmmoText.text = "Ammo ∞";
                break;

            case CurrentWeapon.machineGun:
                AmmoText.text = "Ammo " + machineGunAmmo;
                break;

            case CurrentWeapon.grenadeLauncher:
                AmmoText.text = "Ammo " + grenadeLauncherAmmo;
                break;
        }
    }

    void DisableEffect()
    {
        gunLight.enabled = false;
    }

    // 지금 가진 무기에 맞는 총알을 발사
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

    // 일반 총알 생성
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

    // 머신건 총알 생성
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

    // 유탄 생성
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

    public void GetAmmoBox(int ammo)
    {
        machineGunAmmo += ammo;
        DisplayAmmo();
    }

    public void GetGrenade(int ammo)
    {
        grenadeLauncherAmmo += ammo;
        DisplayAmmo();
    }
}
