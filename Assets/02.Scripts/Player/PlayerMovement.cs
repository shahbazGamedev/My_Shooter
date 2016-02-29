using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 플레이어 이동을 처리

public class PlayerMovement : MonoBehaviour {

    public float moveSpeed = 10.0f;             // 이동 속도
    public float rotSpeed = 40.0f;              // 회전 속도
    public bool isBackStep = false;             // 백스텝 상태 여부

    public Image backStepBtnImg;
    public Image padImg;

    private float h = 0.0f;                     // 좌우 입력 변수
    private float v = 0.0f;                     // 상하 입력 변수
    private Vector3 dir;                        // 입력 방향 벡터
    private bool isTouchJoyStick = false;
    private int joyStickTouchIdx;                    // 가상 조이스틱이 터치됐을 때 터치 주소값 저장

    private Animator anim;
    private Color clearColor = new Color(1f, 1f, 1f, 1f);
    private Color transparentColor = new Color(1f, 1f, 1f, 0.5f);

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
        TouchInput();
#endif

        Move();
        Turn();
        Animating();
    }

    // 안드로이드에서 터치 입력을 처리
    void TouchInput()
    {
        if(Input.touchCount > 0)
        {
            for(int i = 0; i < Input.touchCount; i++)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("VRMoveArea")))
                {
                    // 이동을 위한 패드에선 Moved 또는 Stationary로 계속 누른 상태만 입력으로 받음 
                    if (i == joyStickTouchIdx &&
                        isTouchJoyStick &&
                        Input.GetTouch(i).phase == TouchPhase.Moved ||
                        Input.GetTouch(i).phase == TouchPhase.Stationary)
                    {
                        CheckMovJoyStick(hit, Input.GetTouch(i).position);
                    }
                }

                // 백스텝 모드는 한번 터치한 것만 입력으로 처리
                if(Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("VRPad")))
                    {
                        TouchJoyStick(hit, i);
                        TouchBackStep(hit);
                    }
                }

                if(i == joyStickTouchIdx &&
                   isTouchJoyStick &&
                   Input.GetTouch(i).phase == TouchPhase.Ended)
                {
                    joyStickTouchIdx = -1;
                    isTouchJoyStick = false;
                    SetPadImgAlpha();
                }
            }
        }
    }

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

        Quaternion rot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotSpeed * Time.deltaTime);
    }

    void Animating()
    {
        bool isWalking = dir != Vector3.zero;
        anim.SetBool("IsWalking", isWalking);
    }

    // 백스텝 버튼을 누르면 백스텝 모드 또는 평상 모드로 변한다.
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

    void TouchJoyStick(RaycastHit hit, int touchIdx)
    {
        if (hit.collider.CompareTag("VR_JOYSTICK"))
        {
            isTouchJoyStick = true;
            joyStickTouchIdx = touchIdx;
            SetPadImgAlpha();
        }
    }

    // 터치한 것이 백스텝 버튼인지 확인하고 현재 상태를 바꿈
    void TouchBackStep(RaycastHit hit)
    {
        if (hit.collider.CompareTag("BUTTON_BACKSTEP"))
        {
            isBackStep = !isBackStep;
            SetBackImgAlpha();
        }
    }

    // 백스텝 모드 버튼의 알파값 변경
    void SetBackImgAlpha()
    {
        if (isBackStep)
            backStepBtnImg.color = clearColor;
        else
            backStepBtnImg.color = transparentColor;
    }

    // 조이스틱 패드 이미지 알파값 변경
    void SetPadImgAlpha()
    {
        if (isTouchJoyStick)
            padImg.color = clearColor;
        else
            padImg.color = transparentColor;
    }

    void CheckMovJoyStick(RaycastHit hit, Vector2 posTouch)
    {
        Vector3 direction = hit.point - padImg.transform.position;
        direction.y = 0;
        dir = direction.normalized;
    }
}
