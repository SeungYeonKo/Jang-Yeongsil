using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class InventionSpawnManager : MonoBehaviourPunCallbacks
{
    public GameObject CheugugiPrefab; // 에디터에서 할당할 프리팹

    private bool hasDropped = false;  // 한 번 드랍했는지 여부

    private void Awake()
    {
        Debug.Log("InventionSpawnManager 불러오기 성공");
        DontDestroyOnLoad(this.gameObject); // 씬 전환 시 오브젝트 유지
    }

    private void Start()
    {
        Debug.Log("InventionSpawnManager Start 호출됨");

        // Custom Properties에 변경사항이 있을 때 처리
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("WinnerPlayerNumber"))
        {
            Debug.Log("WinnerPlayerNumber 키가 이미 존재함.");
            HandleWinnerNumber((int)PhotonNetwork.CurrentRoom.CustomProperties["WinnerPlayerNumber"]);
        }
        else
        {
            Debug.LogWarning("WinnerPlayerNumber 키가 아직 존재하지 않음.");
        }
    }

    /*public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("WinnerPlayerNumber"))
        {
            Debug.Log("WinnerPlayerNumber가 업데이트됨.");
            HandleWinnerNumber((int)propertiesThatChanged["WinnerPlayerNumber"]);
        }
    }*/

    private void HandleWinnerNumber(int winnerPlayerNumber)
    {
        if (!hasDropped)
        {
            RainGaugePlayer winner = FindWinnerInScene(winnerPlayerNumber);

            if (winner != null)
            {
                Debug.Log("승자 찾기 성공: " + winner.name);
                Vector3 dropPosition = winner.transform.position + new Vector3(0, 1, 0); // 승자 앞에 Cheugugi 드랍

                // Cheugugi 오브젝트를 로컬에서 생성
                GameObject cheugugiObject = Instantiate(CheugugiPrefab, dropPosition, Quaternion.identity);

                if (cheugugiObject != null)
                {
                    // PhotonView를 추가하고 네트워크 ID 할당
                    PhotonView photonView = cheugugiObject.AddComponent<PhotonView>();
                    photonView.ViewID = PhotonNetwork.AllocateViewID(true); // ViewID 할당

                    Debug.Log("Cheugugi 오브젝트가 성공적으로 생성되었습니다.");

                    // 다른 클라이언트에 이 오브젝트의 생성 정보를 동기화하기 위해 RPC 호출
                    photonView.RPC("SyncCheugugiObject", RpcTarget.OthersBuffered, photonView.ViewID, dropPosition);

                    hasDropped = true;
                }
                else
                {
                    Debug.LogError("Cheugugi 오브젝트 생성에 실패했습니다.");
                }
            }
            else
            {
                Debug.LogWarning("승자를 찾지 못했습니다.");
            }
        }
        else
        {
            Debug.LogWarning("이미 Cheugugi가 드랍되었습니다.");
        }
    }

    private RainGaugePlayer FindWinnerInScene(int playerNumber)
    {
        RainGaugePlayer[] allPlayers = FindObjectsOfType<RainGaugePlayer>();
        foreach (RainGaugePlayer player in allPlayers)
        {
            if (player.MyNum == playerNumber)
            {
                Debug.Log("Winner 찾기 성공: " + player.name);
                return player;
            }
        }
        Debug.LogWarning("Winner 찾기 실패");
        return null;
    }

    // 이 RPC 메서드는 다른 클라이언트에서 오브젝트를 동기화하는 데 사용됩니다.
    [PunRPC]
    public void SyncCheugugiObject(int viewID, Vector3 position, PhotonMessageInfo info)
    {
        // Cheugugi 오브젝트를 다른 클라이언트에서 동일하게 생성
        GameObject cheugugiObject = Instantiate(CheugugiPrefab, position, Quaternion.identity);

        if (cheugugiObject != null)
        {
            // PhotonView를 추가하고 동일한 ViewID를 설정
            PhotonView photonView = cheugugiObject.AddComponent<PhotonView>();
            photonView.ViewID = viewID;

            Debug.Log("다른 클라이언트에서 Cheugugi 오브젝트가 성공적으로 동기화되었습니다.");
        }
        else
        {
            Debug.LogError("다른 클라이언트에서 Cheugugi 오브젝트 생성에 실패했습니다.");
        }
    }
}
