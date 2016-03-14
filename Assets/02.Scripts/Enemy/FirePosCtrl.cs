using UnityEngine;
using System.Collections;

// MageSkeleton의 탄 발사 지점 스크립트

public class FirePosCtrl : MonoBehaviour {

    public GameObject throwObject;                  // 발사되는 탄막

    private Transform playerTr;                     // 플레이어 Transform을 저장

    void Awake()
    {
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

	void OnEnable()
    {
        // 오브젝트가 활성화되면 탄 발사
        CreateDagger();
    }

    // 함수 : CreateDagger
    // 목적 : 플레이어를 향해 오브젝트 풀에 있는 단검을 활성화해 발사
    void CreateDagger()
    {
        Vector3 toPlayerDir = playerTr.position - transform.position;       // 플레이어를 향한 벡터
        toPlayerDir.y = 0;                                                  // 지면과 수평으로 날아가도록 함

        // 오브젝트 풀에서 활성화 되지 않은 단검을 활성화시켜 발사
        foreach (GameObject dagger in BulletManager.instance.daggerPool)
        {
            if (!dagger.activeSelf)
            {
                dagger.transform.position = transform.position;
                dagger.transform.rotation = Quaternion.LookRotation(toPlayerDir);
                dagger.SetActive(true);
                break;
            }
        }
    }
}
