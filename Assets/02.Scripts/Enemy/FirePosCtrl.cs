using UnityEngine;
using System.Collections;

// MageSkeleton의 탄 발사 지점 스크립트

public class FirePosCtrl : MonoBehaviour {

    public GameObject throwObject;                  // 발사되는 탄막

    private Transform playerTr;

    void Awake()
    {
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    // 오브젝트가 활성화되면 탄 발사
	void OnEnable()
    {
        CreateDagger();
    }

    void CreateDagger()
    {
        Vector3 toPlayerDir = playerTr.position - transform.position;       // 플레이어를 향한 벡터
        toPlayerDir.y = 0;                                                  // 수직으로 날아가도록 함

        foreach (GameObject dagger in BulletManager.instance.daggerPool)
        {
            // 활성화 되지 않은 단검을 활성화시켜 발사
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
