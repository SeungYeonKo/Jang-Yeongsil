using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Cinemachine;

public class PlayerMoveAbility : PlayerAbility
{
    public float Speed;
    private float Movespeed = 10f;
    private float RunSpeed = 13f;
    private float sensitivity = 2.0f; // 회전 감도 조정
    private float _rotationX = 0f;
    private float _rotationY = 0f;

    private float NormalJumpPower = 5f; // 점프 힘을 증가
    private float RunningJumpPower = 8f; // 점프 힘을 증가

    private float _JumpPower;
    private float _jumpCooldown = 1.7f; // 점프 쿨타임
    private float _lastJumpTime; // 마지막 점프 시간

    public bool isGrounded;
    public Transform LayerPoint;
    public LayerMask groundMask;
    public float groundDistance = 0.5f;
    public bool _isRunning;

    private Animator _animator;

    Rigidbody rb;
    public Transform CameraRoot;
    Vector3 dir = Vector3.zero;

    SunMiniGame sunMiniGame;
    private ChatGPTManager chatGPTManager;

    private TPSCamera tpsCamera;  // TPSCamera 인스턴스 참조 추가

    private string currentSceneName; // 현재 씬 이름 저장
    private bool useCinemachine = false; // 시네머신 사용 여부

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        // ChatGPTManager 참조를 추가
        chatGPTManager = FindObjectOfType<ChatGPTManager>();

        if (_owner != null && _owner.PhotonView.IsMine)
        {
            GameObject mainCamera = GameObject.FindWithTag("MainCamera");
            if (mainCamera != null)
            {
                tpsCamera = mainCamera.GetComponent<TPSCamera>();  // 인스턴스 가져오기
                if (tpsCamera != null)
                {
                    tpsCamera.target = CameraRoot;
                }
            }
        }

        // 현재 씬 이름을 가져와 저장
        currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "NewRainGauge")
        {
            useCinemachine = true;  // 측우기 씬에서 시네머신 사용
          
        }

        // ClepsydraScene에서만 이동 속도 조정
        SetSpeedForCurrentScene();

        // 씬 로드 이벤트 구독
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnDisable()
    {
        // 씬 로드 이벤트 구독 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 새로운 씬이 로드될 때마다 씬 이름을 업데이트
        currentSceneName = scene.name;

        // ClepsydraScene에서만 이동 속도 조정
        SetSpeedForCurrentScene();
    }

    private void SetSpeedForCurrentScene()
    {
        if (currentSceneName == "ClepsydraScene")
        {
            Debug.Log("플레이어 속도 ClepsydraScene용으로 수정");
            Movespeed = 6f;
            RunSpeed = 8f;
        }
        else
        {
            Debug.Log("플레이어 속도 복구");

            // ClepsydraScene이 아닐 경우 원래 속도로 설정
            Movespeed = 10f;
            RunSpeed = 13f;
        }
    }

    void Update()
    {
        // UI가 활성화되어 있으면 캐릭터 조작을 중지
        if (chatGPTManager != null && chatGPTManager.isUIActive)
        {
            return;
        }

        if (_owner == null || !_owner.PhotonView.IsMine || !enabled)
        {
            return;
        }

        GroundCheck();
        if (_animator != null && Input.GetKeyDown(KeyCode.T))
        {
            _animator.SetTrigger("Punching");
        }

        // 시네머신인 경우에만 기존 방식으로 마우스 입력 받음
        if (useCinemachine)
        {
            MouseInput();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && Time.time >= _lastJumpTime + _jumpCooldown)
        {
            if (_isRunning)
            {
                _JumpPower = RunningJumpPower;
            }
            else
            {
                _JumpPower = NormalJumpPower;
            }
            _animator.SetBool("Jump", true);
            JumpCode();
            _lastJumpTime = Time.time; // 마지막 점프 시간을 현재 시간으로 갱신
        }
    }
    private void MouseInput()
    {
        _rotationX += Input.GetAxis("Mouse X") * sensitivity;
        _rotationY -= Input.GetAxis("Mouse Y") * sensitivity;
        _rotationY = Mathf.Clamp(_rotationY, -90f, 90f); // 상하 회전 제한

        // 마우스 입력에 따른 회전 처리
        CameraRoot.rotation = Quaternion.Euler(_rotationY, _rotationX, 0);
    }

    private void FixedUpdate()
    {
        // UI가 활성화되어 있으면 캐릭터 조작을 중지
        if (chatGPTManager != null && chatGPTManager.isUIActive)
        {
            return;
        }

        if (_owner == null || !_owner.PhotonView.IsMine || !enabled)
        {
            return;
        }

        InputAndDir();
    }

    void InputAndDir()
    {
        dir.x = Input.GetAxis("Horizontal");
        dir.z = Input.GetAxis("Vertical");

        Vector3 direction = Vector3.zero;

        if (_animator != null)
        {
            _animator.SetFloat("Move", Mathf.Clamp01(new Vector3(dir.x, 0f, dir.z).magnitude));
        }

        if (_owner != null && _owner.PhotonView.IsMine)
        {
            if (tpsCamera != null && tpsCamera.FPSview)
            {
                // FPS 모드: 카메라의 회전을 무시하고 플레이어의 현재 회전을 유지한 채 이동
                // 플레이어의 회전은 마우스 입력에 의해서만 변경됨
                direction = transform.forward * dir.z + transform.right * dir.x;
            }
            else
            {
                // TPS 모드: 카메라 방향을 기준으로 이동
                Vector3 forward = Camera.main.transform.forward;
                forward.y = 0;
                forward.Normalize();

                Vector3 right = Camera.main.transform.right;
                right.y = 0;
                right.Normalize();

                direction = (forward * dir.z + right * dir.x).normalized;
            }
        }

        // 캐릭터가 이동할 때만 회전하도록 설정
        if (direction.magnitude >= 0.1f)
        {
            if (!(tpsCamera != null && tpsCamera.FPSview))
            {
                // FPS 모드에서는 회전하지 않음
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f));
            }

            // 이동 로직
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Speed = RunSpeed;
            }
            else
            {
                Speed = Movespeed;
            }

            rb.MovePosition(rb.position + direction * Speed * Time.fixedDeltaTime);
            _isRunning = Input.GetKey(KeyCode.LeftShift);
        }
    }

    private void JumpCode()
    {
        rb.velocity = new Vector3(rb.velocity.x, _JumpPower, rb.velocity.z); // y-속도를 직접 설정
        Debug.Log("스페이스바 누름");
    }

    void GroundCheck()
    {
        RaycastHit hit;
        int defaultLayerMask = LayerMask.GetMask("Default");
        if (Physics.Raycast(LayerPoint.position, Vector3.down, out hit, groundDistance, defaultLayerMask))
        {
            isGrounded = true;
            Physics.gravity = new Vector3(0, -9.81f, 0);
            _animator.SetBool("Jump", false);
        }
        else
        {
            isGrounded = false;
        }
    }

    public void DisableMovement()
    {
        enabled = false;
        rb.velocity = Vector3.zero; // 비활성화할 때 즉시 속도 멈추기
    }

    public void EnableMovement()
    {
        enabled = true;
    }
}
