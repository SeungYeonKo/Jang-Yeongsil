using Photon.Pun;
using System.Linq;
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
        AssignPlayerNumber();
        PhotonNetwork.LocalPlayer.SetCustomProperties
            (new Hashtable { { "PlayerNumber", MyNum }, { "PlayerJarNumber", MyNum } });
        _startpoint = GameObject.Find($"Start{MyNum}");
        MoveStartPosition();
        _jarController = FindObjectOfType<Jar>();
    }

    private void AssignPlayerNumber()
    {
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("PlayerNumber"))
        {
            int assignedNumber = PhotonNetwork.LocalPlayer.ActorNumber; // Use ActorNumber as unique identifier
            Debug.Log($"Assigning PlayerNumber: {assignedNumber} to player: {PhotonNetwork.LocalPlayer.NickName}");
            MyNum = assignedNumber % 4 + 1;
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "PlayerNumber", MyNum } });
        }
        else
        {
            MyNum = (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerNumber"];
        }
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
            Vector3 handPosition = GetHandPosition(MyNum);
            photonView.RPC("RPC_SetJarPosition", RpcTarget.AllBuffered, MyNum, handPosition);
        }
        if (RainGaugeManager.Instance.CurrentGameState == GameState.Loading)
        {
            MoveStartPosition();
        }
    }

    private int GetUniqueRandomNumber()
    {
        return PhotonNetwork.LocalPlayer.ActorNumber;
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
        string firstPlayerName = (string)PhotonNetwork.CurrentRoom.CustomProperties["FirstPlayerName"];
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

    [PunRPC]
    private void RPC_SetJarPosition(int playerNumber, Vector3 position)
    {
        _jarController.SetJarPosition(playerNumber, position);
    }
}
