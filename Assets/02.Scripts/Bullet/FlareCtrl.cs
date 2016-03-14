using UnityEngine;
using System.Collections;

// 탄이 명중했을때 생기는 피탄 효과에 대한 스크립트

public class FlareCtrl : MonoBehaviour {

    public float disableDelay;                              // 파티클이 유지되는 시간

    private ParticleSystem particleSys;                     // 파티클 시스템

    void Awake()
    {
        particleSys = GetComponent<ParticleSystem>();
        disableDelay = particleSys.duration + 0.2f;         // 파티클의 시간보다 0.2f만큼 더 유지
    }

	void OnEnable()
    {
        particleSys.Stop();
        particleSys.Play();
        StartCoroutine(DisableFlare());
    }

    // 함수 : DisableFlare
    // 목적 : disableDelay 만큼 기다렸다 오브젝트 비활성화
    IEnumerator DisableFlare()
    {
        yield return new WaitForSeconds(disableDelay);

        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }
}
