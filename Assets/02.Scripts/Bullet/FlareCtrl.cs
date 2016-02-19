using UnityEngine;
using System.Collections;

// 탄이 명중했을때 생기는 피탄 효과에 대한 스크립트

public class FlareCtrl : MonoBehaviour {

    public float disableDelay;

    private ParticleSystem particleSys;

    void Awake()
    {
        particleSys = GetComponent<ParticleSystem>();
        disableDelay = particleSys.duration + 0.2f;
    }

	void OnEnable()
    {
        particleSys.Stop();
        particleSys.Play();
        StartCoroutine(DisableFlare());
    }

    IEnumerator DisableFlare()
    {
        yield return new WaitForSeconds(disableDelay);

        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }
}
