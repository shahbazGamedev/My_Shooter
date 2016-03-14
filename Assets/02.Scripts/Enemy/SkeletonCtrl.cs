using UnityEngine;
using System.Collections;

// 모든 스켈레톤의 공통된 동작을 처리. SkeletonMageCtrl, SkeletonWarriorCtrl에서 상속

public class SkeletonCtrl : MonoBehaviour {

    public enum CurrentState {
        attack,                             // 공격
        die,                                // 죽음
        idle,                               // 기본
        run,                                // 이동
    };
    public CurrentState curState = CurrentState.idle;

    public float traceDist;                 // 추적 거리
    public float attackDist = 3.0f;         // 공격 인식 거리
    public float attackDelay = 2.0f;        // 공격 쿨타임
    public float rotSpeed = 40.0f;          // 회전 속도
    public float disableDelay = 2.0f;       // 사망시 이 시간만큼 기다렸다 오브젝트 비활성화
    public int score = 0;                   // 처치 시 얻을 수 있는 점수
    public bool isDead = false;             // 사망 여부

    private Transform playerTr;
    private NavMeshAgent nvAgent;
    private Animator anim;

    protected float attackTimer;                        // 공격 쿨타임과 연동된 타이머
    protected bool isFirstEnable = true;                // 초기 OnEnable 호출시 원래 작업 안하고 false가 됨
    protected bool isAttack = false;                    // 공격 애니메이션 실행중이면 true

    protected virtual void Awake()
    {
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        nvAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        attackTimer = attackDelay;
    }

    protected virtual void Update()
    {
        attackTimer += Time.deltaTime;
    }

    protected virtual void OnEnable()
    {
        isDead = false;

        GetComponent<NavMeshAgent>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<CapsuleCollider>().enabled = true;

        PlayerHealth.OnPlayerDie += this.OnPlayerDie;             // 이벤트 함수 추가
        PlayerGameClear.OnGameClear += this.OnPlayerDie;          // 게임 클리어도 플레이어 사망 이벤트와 같은 것을 사용

        StartCoroutine(CheckState());
        StartCoroutine(ActionState());
    }

    void OnDisable()
    {
        PlayerHealth.OnPlayerDie -= this.OnPlayerDie;
        PlayerGameClear.OnGameClear -= this.OnPlayerDie;
    }

    // 함수 : CheckState
    // 목적 : 적의 상태를 계속 체크하고 변경. 사망 시 종료
    IEnumerator CheckState()
    {
        while(!isDead)
        {
            yield return new WaitForSeconds(0.2f);
            float dist = Vector3.Distance(playerTr.position, transform.position);          // 적과 플레이어 사이의 거리
            isAttack = CheckIsAttack();

            // 공격 사거리에 들어옴
            if(dist <= attackDist)
            {
                curState = CurrentState.attack;
            }
            // 추적 사거리에 들어옴
            else if(dist <= traceDist && !isAttack)
            {
                curState = CurrentState.run;
            }
            else
            {
                curState = CurrentState.idle;
            }
        }
    }

    // 함수 : ActionState
    // 목적 : 현재 상태에 맞는 액션 수행. 사망 시 종료
    IEnumerator ActionState()
    {
        while(!isDead)
        {
            switch (curState)
            {
                // 기본 idle 상태
                case CurrentState.idle:
                    nvAgent.Stop();
                    anim.SetBool("IsRunning", false);
                    break;

                // 플레이어 추적
                case CurrentState.run:
                    nvAgent.destination = playerTr.position;
                    nvAgent.Resume();
                    anim.SetBool("IsRunning", true);
                    anim.SetBool("IsAttackReady", false);
                    break;

                // 공격 대기, 공격 쿨타임 되면 공격
                case CurrentState.attack:
                    nvAgent.Stop();
                    anim.SetBool("IsAttackReady", true);
                    if(attackTimer >= attackDelay)
                    {
                        ActionAttack();
                    }
                    break;
            }

            yield return null;
        }
    }

    // 함수 : ActionAttack
    // 목적 : 공격 애니메이션 실행 및 공격 쿨타임 초기화
    void ActionAttack()
    {
        LookPlayer();
        anim.SetTrigger("Attack");
        attackTimer = 0.0f;
    }

    // 함수 : CheckIsAttack
    // 목적 : 공격 애니메이션이 실행중인지 검사하고 결과 반환
    bool CheckIsAttack()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);  // 현재 실행중인 애니메이션 상태
        if (stateInfo.IsName("attack"))
            return true;
        else
            return false;
    }

    // 함수 : LookPlayer
    // 목적 : 적이 플레이어를 바라보게 함
    void LookPlayer()
    {
        Vector3 dir = playerTr.position - transform.position;

        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotSpeed * Time.deltaTime);
    }

    // 함수 : ActionDie
    // 목적 : 사망 처리 및 컴포넌트 비활성화
    //        SkeletonHealth에서 호출하기 위해 public 선언.
    public void ActionDie()
    {
        isDead = true;

        // 게임 시간이 다 되면 더이상 점수를 얻을 수 없다.
        if (!GameManager.instance.isGameTimeUp)
            ScoreManager.instance.GetScore(score);

        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<CapsuleCollider>().enabled = false;
        anim.SetTrigger("Die");

        StartCoroutine(DisableSkeleton());
    }

    // 함수 : DisableSkeleton
    // 목적 : 딜레이만큼 대기하고 오브젝트 비활성화 및 리셋. ActionDie에서 호출
    IEnumerator DisableSkeleton()
    {
        yield return new WaitForSeconds(disableDelay);
        anim.SetTrigger("Reset");
        gameObject.SetActive(false);
        curState = CurrentState.idle;
        GameManager.instance.enemyCount--;      // 전체 몬스터 수 감소
    }

    // 함수 : OnPlayerDie
    // 목적 : 플레이어 사망시 호출되는 이벤트 함수.
    //        게임 클리어 됐을때도 이 함수가 호출
    //        몬스터의 동작을 정지
    void OnPlayerDie()
    {
        StopAllCoroutines();
        nvAgent.Stop();
        anim.SetTrigger("GameOver");
    }
}
