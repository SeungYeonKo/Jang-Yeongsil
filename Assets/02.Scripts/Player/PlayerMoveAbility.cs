using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMoveAbility : PlayerAbility
{
    public float Speed;
    private float Movespeed = 10f;
    private float RunSpeed = 13f;

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
                TPSCamera tpsCamera = mainCamera.GetComponent<TPSCamera>();
                if (tpsCamera != null)
                {
                    tpsCamera.target = CameraRoot;
                }
            }
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

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && Time.time >= _lastJumpTime + _jumpCooldown )
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
        Vector3 direction = new Vector3(dir.x, 0f, dir.z);
        float movementMagnitude = direction.magnitude;

        if (_animator != null)
        {
            _animator.SetFloat("Move", Mathf.Clamp01(movementMagnitude));
        }

        // 카메라의 방향을 기준으로 이동 방향 설정
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0; // 수직 방향 제거하여 평면 이동만 계산
        forward.Normalize();

        Vector3 right = Camera.main.transform.right;
        right.y = 0; // 수직 방향 제거하여 평면 이동만 계산
        right.Normalize();

        // 카메라 방향을 기준으로 이동 방향을 설정
        direction = (forward * dir.z + right * dir.x).normalized;

        // 캐릭터가 이동할 때만 회전하도록 설정
        if (direction.magnitude >= 0.1f)
        {
            // 캐릭터의 목표 회전 설정
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f));

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
