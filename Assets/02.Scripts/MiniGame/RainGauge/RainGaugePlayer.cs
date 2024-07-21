using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RainGaugePlayer : MonoBehaviourPunCallbacks
{
    public bool isReady = false;
    public int MyNum;
    private JarScore _jarScore;
    private bool _isFinished = false;
    private Jar _jarController;
    private GameObject _startpoint;

    public Transform leftHandTransform; 
    public Transform rightHandTransform;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name != "RainGauge")
        {
            this.enabled = false;
            return;
        }
        if (!photonView.IsMine) return;
    }

    private void Start()
    {
        MyNum = GetUniqueRandomNumber();
        PhotonNetwork.LocalPlayer.SetCustomProperties
            (new Hashtable { { "PlayerNumber", MyNum }, { "PlayerJarNumber", MyNum } });
        _startpoint = GameObject.Find($"Start{MyNum}");
        MoveStartPosition();
        _jarController = FindObjectOfType<Jar>();
    }
    public void MoveStartPosition()
    {
        transform.position = _startpoint.transform.position;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            SetReadyStateOnInput();
        }
        if (RainGaugeManager.Instance.CurrentGameState == GameState.Loading)
        {
            MoveStartPosition();
        }
        if (RainGaugeManager.Instance.CurrentGameState == GameState.Go
            || RainGaugeManager.Instance.CurrentGameState == GameState.Over)
        {
            Vector3 handPosition = GetHandPosition(MyNum);
            _jarController.SetJarPosition(MyNum, handPosition);
        }
    }

    private int GetUniqueRandomNumber()
    {
        int num = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log($"playernum = {num}");
        return num;
    }

    private void SetReadyStateOnInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            isReady = !isReady;
            UpdateReadyState(isReady);
        }
    }

    private void UpdateReadyState(bool readyState)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "IsReady_RainGauge", readyState } });
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        string firstPlayerName = (string)PhotonNetwork.CurrentRoom.CustomProperties["FirstPlayerName"]; // JarScore에서 저장
        if (firstPlayerName != null)
        {
            if (RainGaugeManager.Instance.CurrentGameState == GameState.Over)
            {
                if (!_isFinished)
                {
                    Animator animator = GetComponent<Animator>();
                    if (photonView.IsMine)
                    {
                        if (firstPlayerName == photonView.Owner.NickName)
                        {
                            //UI_GameOver.Instance.CheckFirst();
                            animator.SetBool("Win", true);
                        }
                        else
                        {
                            //UI_GameOver.Instance.CheckLast();
                            animator.SetBool("Sad", true);
                        }
                    }
                    _isFinished = true;
                }
            }
        }
        else
        {
            return;
        }
    }

    public Vector3 GetHandPosition(int playerNumber)
    {
        return rightHandTransform.position;
    }
}
