using Photon.Pun;
using UnityEngine;

public class PlayerAnimatorSync : MonoBehaviourPun, IPunObservable
{
    private Animator animator;
    private PhotonAnimatorView photonAnimatorView;

    private ChatGPTManager chatGPTManager;

    private float move;
    private bool run;
    private bool runJump;
    private bool walk;
    private bool jump;
    private bool win;
    private bool sad;
    private bool attack;
    private bool attack2;
    private bool flyingAttack;
    private bool dance1;
    private bool dance2;
    private bool dance3;

    private bool isDancing;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        photonAnimatorView = GetComponent<PhotonAnimatorView>();

        // ChatGPTManager 참조를 추가
        chatGPTManager = FindObjectOfType<ChatGPTManager>();

        photonAnimatorView.SetParameterSynchronized("Move", PhotonAnimatorView.ParameterType.Float, PhotonAnimatorView.SynchronizeType.Discrete);
        photonAnimatorView.SetParameterSynchronized("Run", PhotonAnimatorView.ParameterType.Bool, PhotonAnimatorView.SynchronizeType.Discrete);
        // 나머지 파라미터 동기화 설정
    }

    private void Update()
    {
        // UI가 활성화되어 있으면 애니메이션 중지
        if (chatGPTManager != null && chatGPTManager.isUIActive)
        {
            // 모든 애니메이션 파라미터를 초기화하여 애니메이션 중지
            animator.SetFloat("Move", 0);
            animator.SetBool("Run", false);
            animator.SetBool("RunJump", false);
            animator.SetBool("Walk", false);
            animator.SetBool("Jump", false);
            animator.SetBool("Win", false);
            animator.SetBool("Sad", false);
            animator.SetBool("Attack", false);
            animator.SetBool("Attack2", false);
            animator.SetBool("FlyingAttack", false);
            ResetDanceAnimations(); // 춤 애니메이션도 중지
            return;
        }

        if (photonView.IsMine)
        {
            HandleInput();
        }
        else
        {
            // 네트워크에서 받은 애니메이션 상태를 업데이트
            animator.SetFloat("Move", move);
            animator.SetBool("Run", run);
            animator.SetBool("RunJump", runJump);
            animator.SetBool("Walk", walk);
            animator.SetBool("Jump", jump);
            animator.SetBool("Win", win);
            animator.SetBool("Sad", sad);
            animator.SetBool("Attack", attack);
            animator.SetBool("Attack2", attack2);
            animator.SetBool("FlyingAttack", flyingAttack);
            animator.SetBool("Dance1", dance1);
            animator.SetBool("Dance2", dance2);
            animator.SetBool("Dance3", dance3);
        }
    }

    private void HandleInput()
    {
        move = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        animator.SetFloat("Move", move);
        animator.SetBool("Walk", move != 0 || horizontal != 0);

        if (Input.GetMouseButtonDown(1))
        {
            attack = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            attack = false;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            run = true;
        }
        else
        {
            run = false;
        }

        animator.SetBool("Run", run);

        if (Input.GetKeyDown(KeyCode.F))
        {
            int randomValue = UnityEngine.Random.Range(1, 4);
            ResetDanceAnimations();
            animator.SetBool($"Dance{randomValue}", true);
            isDancing = true;
        }

        if (isDancing == true && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space)))
        {
            isDancing = false;
            ResetDanceAnimations();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(animator.GetFloat("Move"));
            stream.SendNext(animator.GetBool("Run"));
            stream.SendNext(animator.GetBool("RunJump"));
            stream.SendNext(animator.GetBool("Walk"));
            stream.SendNext(animator.GetBool("Jump"));
            stream.SendNext(animator.GetBool("Win"));
            stream.SendNext(animator.GetBool("Sad"));
            stream.SendNext(animator.GetBool("Attack"));
            stream.SendNext(animator.GetBool("FlyingAttack"));
        }
        else
        {
            move = (float)stream.ReceiveNext();
            run = (bool)stream.ReceiveNext();
            runJump = (bool)stream.ReceiveNext();
            walk = (bool)stream.ReceiveNext();
            jump = (bool)stream.ReceiveNext();
            win = (bool)stream.ReceiveNext();
            sad = (bool)stream.ReceiveNext();
            attack = (bool)stream.ReceiveNext();
            flyingAttack = (bool)stream.ReceiveNext();
        }
    }

    private void ResetDanceAnimations()
    {
        animator.SetBool("Dance1", false);
        animator.SetBool("Dance2", false);
        animator.SetBool("Dance3", false);
    }
}
