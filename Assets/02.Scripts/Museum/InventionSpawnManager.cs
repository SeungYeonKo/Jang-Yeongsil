// InventionSpawnManager.cs
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class InventionSpawnManager : MonoBehaviourPunCallbacks
{
    public string CheugugiPrefabName = "Cheugugi"; // Resources 폴더에 저장된 프리팹 이름
    private GameObject CheugugiPrefab;
    private bool hasDropped = false;  // 한 번 드랍했는지 여부

    private void Awake()
    {
        Debug.Log("InventionSpawnManager 불러오기 성공");
        DontDestroyOnLoad(this.gameObject); // 씬 전환 시 오브젝트 유지
    }

    private void Start()
    {
        Debug.Log("InventionSpawnManager Start 호출됨");

        // 프리팹을 Resources에서 동적으로 로드
        CheugugiPrefab = Resources.Load<GameObject>(CheugugiPrefabName);
        if (CheugugiPrefab == null)
        {
            Debug.LogError("CheugugiPrefab을 찾을 수 없습니다. Resources 폴더에 프리팹이 있는지 확인하세요.");
            return;
        }

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

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("WinnerPlayerNumber"))
        {
            Debug.Log("WinnerPlayerNumber가 업데이트됨.");
            HandleWinnerNumber((int)propertiesThatChanged["WinnerPlayerNumber"]);
        }
    }

    private void HandleWinnerNumber(int winnerPlayerNumber)
    {
        if (!hasDropped)
        {
            RainGaugePlayer winner = FindWinnerInScene(winnerPlayerNumber);

            if (winner != null)
            {
                Debug.Log("승자 찾기 성공: " + winner.name);
                StartCoroutine(DropCheugugiAfterDelay(winner.transform.position)); // 2초 후에 드랍
                hasDropped = true;
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

    private IEnumerator DropCheugugiAfterDelay(Vector3 position)
    {
        Debug.Log("DropCheugugiAfterDelay 실행");
        yield return new WaitForSeconds(2f); // 2초 대기

        if (!hasDropped) // 이미 드랍한 적이 없는 경우에만 드랍
        {
            Debug.Log("Cheugugi 드랍 시작");
            DropCheugugi(position);
            hasDropped = true;
        }
        else
        {
            Debug.LogWarning("이미 Cheugugi가 드랍되었습니다.");
        }
    }

    private void DropCheugugi(Vector3 position)
    {
        if (CheugugiPrefab != null)
        {
            Debug.Log("CheugugiPrefab을 생성 중...");
            Vector3 dropPosition = position + new Vector3(0, 1, 0); // 승자 앞에 발명품을 드랍

            // 프리팹 인스턴스화
            GameObject instantiatedObject = Instantiate(CheugugiPrefab, dropPosition, Quaternion.identity);
            Debug.Log("CheugugiPrefab 생성 완료");

            // 오브젝트 활성화
            instantiatedObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Cheugugi prefab is not assigned.");
        }
    }
}
