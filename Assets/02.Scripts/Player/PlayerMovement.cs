using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 플레이어 이동 관련 처리
// 가상 패드의 이동과 백스텝 모드 변환은 여기서

public class PlayerMovement : MonoBehaviour {

    public float moveSpeed = 10.0f;             // 이동 속도
    public float rotSpeed = 40.0f;              // 회전 속도
    public bool isBackStep = false;             // 백스텝 상태 여부

    public Image backStepBtnImg;
    public Image padImg;

    private float h = 0.0f;                     // 좌우 입력 변수 (에디터에서만 사용)
    private float v = 0.0f;                     // 상하 입력 변수 (에디터에서만 사용)

    private Vector3 dir;                        // 입력 방향 벡터
    private bool isTouchJoyStick = false;       // 가상 조이스틱 터치 여부
    private int joyStickTouchIdx;               // 가상 조이스틱이 터치됐을 때 터치 주소값 저장

    private Animator anim;
    private Color clearColor = new Color(1f, 1f, 1f, 1f);           // 완전 불투명한 컬러
    private Color transparentColor = new Color(1f, 1f, 1f, 0.5f);   // 반투명한 컬러

    void Awake ()
    {
        anim = GetComponent<Animator>();

        if(backStepBtnImg)
            SetBackImgAlpha();

        SetPadImgAlpha();
    }
	
	void Update ()
    {
        dir = Vector3.zero;

#if UNITY_EDITOR
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        dir = (Vector3.forward * v) + (Vector3.right * h);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        ClickBackStepPC(ray);
#endif

#if UNITY_ANDROID
        TouchInput(); // 터치 입력
#endif

        Move();       // 이동
        Turn();       // 회전
        Animating();  // 이동 애니메이션
    }

    // 함수 : TouchInput
    // 목적 : 안드로이드에서 터치 입력을 처리
    void TouchInput()
    {
        if(Input.touchCount > 0)
        {
            // 멀티 터치 처리를 위해 입력된 모든 터치에 대해서 확인한다
            for(int i = 0; i < Input.touchCount; i++)
            {
                // 터치 위치로 레이 발사
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                RaycastHit hit;

                // 이동 조작을 위한 영역을 터치했는지 확인
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("VRMoveArea")))
                {
                    // 이동을 위한 패드에선 Moved 또는 Stationary로 계속 누른 상태만 입력으로 받음 
                    if (i == joyStickTouchIdx &&
                        isTouchJoyStick &&
                        Input.GetTouch(i).phase == TouchPhase.Moved ||
                        Input.GetTouch(i).phase == TouchPhase.Stationary)
                    {
                        CheckMoveJoyStick(hit, Input.GetTouch(i).position); // 이동 방향 계산
                    }
                }

                // 가상 조이스틱 터치 시작, 백스텝 모드는 한번 터치한 것만 입력으로 처리
                if(Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("VRPad")))
                    {
                        TouchJoyStick(hit, i);
                        TouchBackStep(hit);
                    }
                }
                
                // 이동 터치 조작중에 손을 뗐을 때 처리
                if(i == joyStickTouchIdx &&
                   isTouchJoyStick &&
                   Input.GetTouch(i).phase == TouchPhase.Ended)
                {
                    joyStickTouchIdx = -1;
                    isTouchJoyStick = false; // 이제 가상 조이스틱을 터치한 상태가 아니다
                    SetPadImgAlpha();
                }
            }
        }
    }

    // 함수 : Move
    // 목적 : 플레이어 이동을 처리
    void Move()
    {
        float speed;
#if UNITY_ANDROID
        // 조이스틱 패드 입력이 안 된 상태면 정지
        if (!isTouchJoyStick)
            dir = Vector3.zero;
#endif

        // 백스텝 상태일 때는 이동속도가 줄어든다.
        if (isBackStep)
            speed = moveSpeed - 2.5f;
        else
            speed = moveSpeed;

        transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }

    // 함수 : Turn
    // 목적 : 플레이어 회전을 처리
    void Turn()
    {
        if (dir == Vector3.zero)
            return;

        Vector3 direction;

        // 백스텝 상태일 때는 반대 방향을 본다.
        if (isBackStep)
            direction = -dir;
        else
            direction = dir;

        // 전방 또는 후방을 바라보도록 회전
        Quaternion rot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotSpeed * Time.deltaTime);
    }

    // 함수 : Animating
    // 목적 : 플레이어 이동 애니메이션 처리
    void Animating()
    {
        bool isWalking = dir != Vector3.zero; // 플레이어가 움직이는 중이면 true 아니면 false
        anim.SetBool("IsWalking", isWalking); // 애니메이션 실행
    }

    // 함수 : ClickBackStepPC
    // 목적 : PC 버전을 위한 함수
    //        백스텝 버튼을 클릭하면 백스텝 모드 또는 평상 모드로 변한다.
    void ClickBackStepPC(Ray ray)
    {
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.collider.CompareTag("BUTTON_BACKSTEP"))
                {
                    isBackStep = !isBackStep;

                    SetBackImgAlpha();
                }
            }

        }
    }

    // 함수 : TouchJoyStick
    // 목적 : 터치한 것이 가상 조이스틱인지 확인하고 터치 주소를 저장, 이미지의 알파값 변경
    //        TouchInput에서 호출
    void TouchJoyStick(RaycastHit hit, int touchIdx)
    {
        if (hit.collider.CompareTag("VR_JOYSTICK"))
        {
            isTouchJoyStick = true;
            joyStickTouchIdx = touchIdx;
            SetPadImgAlpha();
        }
    }

    // 함수 : TouchBackStep
    // 목적 : 터치한 것이 백스텝 버튼인지 확인하고 현재 상태를 바꿈
    void TouchBackStep(RaycastHit hit)
    {
        if (hit.collider.CompareTag("BUTTON_BACKSTEP"))
        {
            isBackStep = !isBackStep;
            SetBackImgAlpha();
        }
    }

    // 함수 : SetBackImgAlpha
    // 목적 : 백스텝 모드 버튼의 알파값 변경
    void SetBackImgAlpha()
    {
        if (isBackStep)
            backStepBtnImg.color = clearColor;
        else
            backStepBtnImg.color = transparentColor;
    }

    // 함수 : SetPadImgAlpha
    // 목적 : 조이스틱 패드 이미지 알파값 변경
    void SetPadImgAlpha()
    {
        if (isTouchJoyStick)
            padImg.color = clearColor;
        else
            padImg.color = transparentColor;
    }

    // 함수 : CheckMovJoyStick
    // 목적 : 이동 방향을 계산하기 위한 함수
    //        가상 조이스틱의 위치에서 터치 위치를 향한 벡터를 이동 방향으로 결정한다
    //        TouchInput에서 호출
    void CheckMoveJoyStick(RaycastHit hit, Vector2 posTouch)
    {
        Vector3 direction = hit.point - padImg.transform.position;
        direction.y = 0;
        dir = direction.normalized;
    }
}
