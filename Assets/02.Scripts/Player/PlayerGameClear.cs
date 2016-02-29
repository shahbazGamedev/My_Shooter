using UnityEngine;
using System.Collections;

public class PlayerGameClear : MonoBehaviour {

    // 게임 클리어 시 호출될 이벤트
    public delegate void GameClearHandler();
    public static event GameClearHandler OnGameClear;

    private PlayerHealth pHealth;
    private PlayerMovement pMovement;
    private PlayerShooting pShooting;

    void Awake()
    {
        pHealth = GetComponent<PlayerHealth>();
        pMovement = GetComponent<PlayerMovement>();
        pShooting = GetComponentInChildren<PlayerShooting>();
    }

    void OnTriggerEnter(Collider coll)
    { 
        if (coll.CompareTag("GAME_CLEAR")) // 게임 클리어
        {
            OnGameClear();              // 게임 클리어 이벤트 발생
            pHealth.enabled = false;    // 플레이어 컴포넌트 비활성화
            pMovement.enabled = false;
            pShooting.enabled = false;
        }
    }
}
