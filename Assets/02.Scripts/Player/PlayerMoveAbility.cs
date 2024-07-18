using JetBrains.Annotations;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMoveAbility : PlayerAbility
{
    public float Speed;
    private float Movespeed = 3f;
    private float RunSpeed = 5f;

    private float NormalJumpPower = 5f; // 점프 힘을 증가
    private float RunningJumpPower = 8f; // 점프 힘을 증가

    private float _JumpPower;

    public bool isGrounded;
    public Transform LayerPoint;
    public LayerMask groundMask;
    public float groundDistance = 0.5f;
    public bool _isRunning;

    private Animator _animator;

    Rigidbody rb;
    public Transform CameraRoot;
    Vector3 dir = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

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
        if (_owner == null || !_owner.PhotonView.IsMine)
        {
            return;
        }
        GroundCheck();
        if (_animator != null && Input.GetKeyDown(KeyCode.T))
        {
            _animator.SetTrigger("Punching");
        }
    }

    private void FixedUpdate()
    {
        if (_owner == null || !_owner.PhotonView.IsMine)
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

        rb.velocity = new Vector3(direction.x, rb.velocity.y, direction.z);

        if (dir != Vector3.zero)
        {
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0;
            direction = (forward.normalized * dir.z + Camera.main.transform.right * dir.x).normalized;

            var a = direction;
            a.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(a);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f));
        }

        direction.y = 0f;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Speed = RunSpeed;
            rb.MovePosition(rb.position + direction * Speed * Time.fixedDeltaTime);
            _isRunning = true;
        }
        else
        {
            Speed = Movespeed;
            rb.MovePosition(rb.position + direction * Speed * Time.fixedDeltaTime);
            _isRunning = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            if (_isRunning)
            {
                _JumpPower = RunningJumpPower;
            }
            else
            {
                _JumpPower = NormalJumpPower;
            }
            JumpCode();
        }
    }

    private void JumpCode()
    {
        rb.velocity = new Vector3(rb.velocity.x, _JumpPower, rb.velocity.z); // y-속도를 직접 설정
        _animator.SetBool("Jump", true);
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
        }
        else
        {
            isGrounded = false;
            _animator.SetBool("Jump", false);
        }
    }
}
