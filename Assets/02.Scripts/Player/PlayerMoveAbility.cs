using JetBrains.Annotations;
using Photon.Pun;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerMoveAbility : PlayerAbility
{
    public float Speed;
    private float Movespeed = 3f;
    private float RunSpeed = 5f;

    private float NormalJumpPower = 2;
    private float RunningJumpPower = 4;
    private float JumpPower = 3f;

    public int JumpCount;
    private int MaxJumpCount = 1;

    private float _JumpPower;

    public bool isGrounded;

    public bool _isRunning;

    private bool _isJumping;
    private bool _isRunningJumping;

    public Transform LayerPoint;
    private Animator _animator;

    Rigidbody rb;
    public Transform CameraRoot;
    Vector3 dir = Vector3.zero;

    [Header("Boxcast Property")]
    [SerializeField] private Vector3 boxSize;
    [SerializeField] private float maxDistance;

    [Header("Debug")]
    [SerializeField] private bool drawGizmo;

    public GroundCecker groundChecker;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        groundChecker = GetComponent<GroundCecker>();

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

        if (groundChecker != null)
        {
            isGrounded = groundChecker.IsGrounded();
        }
        else
        {
            Debug.LogWarning("GroundChecker is not assigned.");
        }

        JumpCounter();

        if (_animator != null && Input.GetKeyDown(KeyCode.T))
        {
            _animator.SetTrigger("Punching");
        }

        if (JumpCount >= MaxJumpCount)
        {
            JumpCount = MaxJumpCount;
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
            Speed = RunSpeed * 2;
            rb.MovePosition(rb.position + direction * Speed * Time.fixedDeltaTime);
            _isRunning = true;
        }
        else
        {
            Speed = Movespeed * 2;
            rb.MovePosition(rb.position + direction * Speed * Time.fixedDeltaTime);
            _isRunning = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            _JumpPower = JumpPower;
            JumpCount -= 1;
            JumpCode();
        }
    }

    public void Jump(float jumpPower)
    {
        rb.AddForce((Vector3.up * jumpPower) / 2f, ForceMode.Impulse);
    }

    private void JumpCode()
    {
        rb.AddForce((Vector3.up * _JumpPower) / 2f, ForceMode.Impulse);
    }

    void JumpCounter()
    {
        if (isGrounded && JumpCount < 1)
        {
            JumpCount += 1;
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmo) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(transform.position - transform.up * maxDistance, boxSize);
    }
}


