using Cinemachine;
using JetBrains.Annotations;
using Photon.Pun;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.UI.GridLayoutGroup;

public class PlayerMoveAbility : PlayerAbility
{
    public float Speed;
    private float Movespeed = 3f;
    private float RunSpeed = 5f;


    private float NormalJumpPower = 2;
    private float RunningJumpPower = 4;

    public int JumpCount;
    private int MaxJumpCount = 1;

    private float _JumpPower;

    public bool isGrounded;		// ���� ���ִ��� üũ�ϱ� ���� bool��
    public LayerMask LayerMask;	// ���̾��ũ ����
    public float groundDistance = 0.4f;		// Ray�� ���� �˻��ϴ� �Ÿ�

    public bool _isRunning;

    private bool _isJumping;
    private bool _isRunningJumping;

    public Transform LayerPoint;
    private Animator _animator;
    private bool _animationEnded;

    Rigidbody rb;
    public Transform CameraRoot;
    Vector3 dir = Vector3.zero;

    private CinemachineFreeLook cinemachineCamera;

    //public ParticleSystem WalkVFX;
    //public ParticleSystem JumpVFX;
    private ParticleSystem[] walkVFX; // Walk VFX �迭
    private int currentVFXIndex = 0; // ���� ��� ���� Walk VFX �ε���
    private float vfxTimer = 0;

    private bool _isFallGuysScene = false; // �������� ������ Ȯ��
    private bool _isTowerClimbScene = false;
    private bool _isBattleTileScene = false;

    private string _sceneName;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _sceneName = SceneManager.GetActiveScene().name;

        _isFallGuysScene = SceneManager.GetActiveScene().name == "FallGuysScene";
        _isTowerClimbScene = SceneManager.GetActiveScene().name == "TowerClimbScene";
        _isBattleTileScene = SceneManager.GetActiveScene().name == "BattleTileScene";

        if (_owner.PhotonView.IsMine && !_isTowerClimbScene && !_isBattleTileScene)
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

        ParticleSystem[] allParticleSystems = GetComponentsInChildren<ParticleSystem>();
        walkVFX = System.Array.FindAll(allParticleSystems, ps => ps.gameObject.name.StartsWith("Walk"));
        for (int i = 0; i < walkVFX.Length; i++)
        {
            walkVFX[i].gameObject.SetActive(false);
        }
    }

    // Ű �Է°� �̵����� ���
    void Update()
    {
        if (!_owner.PhotonView.IsMine)
        {
            return;
        }

        GroundCheck();
        JumpCounter();

        if (Input.GetKeyDown(KeyCode.T))
        {
            _animator.SetTrigger("Punching");
        }

        if (JumpCount >= MaxJumpCount)
        {
            JumpCount = MaxJumpCount;
        }

        if (this.isGrounded)
        {
            vfxTimer = 0;
        }
    }

    private void FixedUpdate()
    {
        if (!_owner.PhotonView.IsMine)
        {
            return;
        }
        if (_sceneName == "BattleTileWinScene")
        {
            this.transform.position = new Vector3(0, 10.9f, -63);
            this.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (_sceneName == "FallGuysWinScene")
        {
            this.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (_sceneName == "TowerClimbWinScene")
        {
            this.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        if (_sceneName.EndsWith("WinScene"))
        {
            return;
        }
        if (_isFallGuysScene)
        {
            if (FallGuysManager.Instance._currentGameState == GameState.Loading)
            { return; }
        }
        else if (_isBattleTileScene)
        {
            if (BattleTileManager.Instance.CurrentGameState == GameState.Loading)
            { return; }
        }
        InputAndDir();
    }

    // Ű �Է°� �׿� ���� �̵������� ����ϴ� �Լ�
    void InputAndDir()
    {
        // Ű �Է¿� ���� ���� ���� ����
        dir.x = Input.GetAxis("Horizontal");   // x�� ���� Ű �Է�
        dir.z = Input.GetAxis("Vertical");     // z�� ���� Ű �Է�
        Vector3 direction = new Vector3(dir.x, 0f, dir.z);
        float movementMagnitude = direction.magnitude;

        // �̵� �ִϸ��̼� ����
        _animator.SetFloat("Move", Mathf.Clamp01(movementMagnitude));
        //Instantiate(WalkVFX, dir, Quaternion.identity);

        // ���� y�� �ӵ��� �����ϸ鼭 ���ο� �������� �ӵ� ����
        rb.velocity = new Vector3(direction.x, rb.velocity.y, direction.z);

        if (dir != Vector3.zero)   // Ű �Է��� �ִ� ���
        {
            // ī�޶��� �� ������ �������� �̵� ���� ����
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0;
            direction = (forward.normalized * dir.z + Camera.main.transform.right * dir.x).normalized;

            var a = direction;
            a.y = 0f;
            // �̵� �������� ĳ���� ȸ��
            Quaternion targetRotation = Quaternion.LookRotation(a);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f));

            // �ȱ� �ִϸ��̼� ����
            _animator.SetBool("Walk", true);
        }
        else // Ű �Է��� ���� ���
        {
            _animator.SetBool("Walk", false);
        }

        direction.y = 0f;

        // �޸��� ���ο� ���� �̵� �ӵ� �� �ִϸ��̼� ����
        if (_isFallGuysScene || Input.GetKey(KeyCode.LeftShift))
        {

            Speed = RunSpeed * 2;

            //Debug.Log(rb.position + direction * Speed * Time.fixedDeltaTime);

            rb.MovePosition(rb.position + direction * Speed * Time.fixedDeltaTime);
            _isRunning = true;

            _animator.SetBool("Run", true);
            //PlayWalkVFX();
        }
        else
        {
            Speed = Movespeed * 2;

            //Debug.Log(rb.position + direction * Speed * Time.fixedDeltaTime);
            rb.MovePosition(rb.position + direction * Speed * Time.fixedDeltaTime);
            _isRunning = false;

            _animator.SetBool("Run", false);
        }

        if (PhotonNetwork.CurrentRoom.Name == "MiniGame1")
        {
            Vector3 newPosition = transform.position;
            //newPosition.x = Mathf.Max(-7.7f, Mathf.Min(7.6f, newPosition.x));
            //newPosition.z = Mathf.Max(0f, Mathf.Min(13.6f, newPosition.z));
            newPosition.x = Mathf.Max(-8f, Mathf.Min(8f, newPosition.x));
            newPosition.z = Mathf.Max(-0.6f, Mathf.Min(14f, newPosition.z));
            transform.position = newPosition;
        }

        if (_isTowerClimbScene)
        {
            _JumpPower = 6;
            if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.V)) && isGrounded)
            {
                JumpCode();
            }

        }
        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.V)) && isGrounded && !_isTowerClimbScene) 	// IsGrounded�� true�� ���� ������ �� �ֵ���
        {
            if (_isBattleTileScene)
            { return; }
            if (_isRunning)
            {
                _JumpPower = RunningJumpPower;
            }
            else
            {
                _JumpPower = NormalJumpPower;
            }
            JumpCount -= 1;
            JumpCode();
        }





    }


    public void Jump(float jumpPower)
    {
        rb.AddForce((Vector3.up * jumpPower) / 2f, ForceMode.Impulse);
        _animator.SetBool("Jump", true);
        //Instantiate(JumpVFX, transform.position, Quaternion.identity);

        if (photonView.IsMine)
        {
            PlayWalkVFX();
        }
        vfxTimer -= Time.deltaTime;
    }

    private void JumpCode()
    {
        rb.AddForce((Vector3.up * _JumpPower) / 2f, ForceMode.Impulse);
        _animator.SetBool("Jump", true);
        //Instantiate(JumpVFX, transform.position, Quaternion.identity);

        if (photonView.IsMine)
        {
            PlayWalkVFX();
        }
        vfxTimer -= Time.deltaTime;
    }

    // ���� ���� ����
    void JumpCounter()
    {

        if (isGrounded && JumpCount < 1)
        {
            JumpCount += 1;
            // �߰� ���� ����
        }
    }

    // ���� �ִ��� �˻��ϴ� �Լ�
    void GroundCheck()
    {
        RaycastHit hit;
        // Default ���̾ ���Ե� ���̾� ����ũ ����
        int defaultLayerMask = LayerMask.GetMask("Default");

        // �÷��̾��� ��ġ����, �Ʒ���������, groundDistance ��ŭ ray�� ����, Default ���̾ �ִ��� �˻�
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

    void PlayWalkVFX()
    {
        if (walkVFX.Length == 0) return;

        if (vfxTimer <= 0)
        {
            // ���� Ȱ��ȭ�� VFX ������Ʈ�� ��Ȱ��ȭ
            if (currentVFXIndex >= 0 && currentVFXIndex < walkVFX.Length)
            {
                walkVFX[currentVFXIndex].gameObject.SetActive(false);
            }

            // ���� VFX ������Ʈ�� Ȱ��ȭ
            currentVFXIndex = (currentVFXIndex + 1) % walkVFX.Length;
            walkVFX[currentVFXIndex].gameObject.SetActive(true);

            // VFX Ȱ��ȭ �̺�Ʈ ����
            photonView.RPC("ActivateVFX", RpcTarget.Others, currentVFXIndex);

            vfxTimer = 1; // Ÿ�̸� �缳��
        }
        else
        {
            vfxTimer -= Time.deltaTime;
        }
    }

    [PunRPC]
    void ActivateVFX(int vfxIndex)
    {
        if (vfxIndex >= 0 && vfxIndex < walkVFX.Length)
        {
            // ��� VFX ��Ȱ��ȭ
            foreach (var vfx in walkVFX)
            {
                vfx.gameObject.SetActive(false);
            }

            // ������ VFX Ȱ��ȭ
            walkVFX[vfxIndex].gameObject.SetActive(true);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Ÿ�̸ӿ� �ε����� ����
            stream.SendNext(vfxTimer);
            stream.SendNext(currentVFXIndex);
        }
        else
        {
            // Ÿ�̸ӿ� �ε����� ����
            vfxTimer = (float)stream.ReceiveNext();
            currentVFXIndex = (int)stream.ReceiveNext();
        }
    }
}
