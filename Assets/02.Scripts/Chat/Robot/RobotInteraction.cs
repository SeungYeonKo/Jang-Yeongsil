using UnityEngine;

public class RobotInteraction : MonoBehaviour
{
    public GameObject robotUI;
    public bool _player = false;
    public GameObject RobotText;

    private void Start()
    {
        // 처음 시작할 때 UI와 로봇 텍스트 비활성화
        robotUI.SetActive(false);
        RobotText.SetActive(false);
    }

    private void Update()
    {
        // _player가 true일 때, Tab 키를 눌러 UI를 활성화/비활성화 전환
        if (_player && Input.GetKeyDown(KeyCode.Tab))
        {
            robotUI.SetActive(!robotUI.activeSelf); // Tab 키로 활성화/비활성화 전환
        }

        // 플레이어가 근처에 있으면 로봇 텍스트 활성화
        if (_player)
        {
            RobotText.SetActive(true);
        }
        else
        {
            RobotText.SetActive(false);
        }
    }

    // 플레이어가 로봇의 트리거 범위에 들어오면 _player를 true로 설정
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player = true;
        }
    }

    // 플레이어가 로봇의 트리거 범위에서 벗어나면 _player를 false로 설정하고 UI를 비활성화
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player = false;
            robotUI.SetActive(false); // 플레이어가 떠날 때 UI를 비활성화
        }
    }
}
