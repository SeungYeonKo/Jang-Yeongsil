using UnityEngine;

public class RobotInteraction : MonoBehaviour
{
    public GameObject robotUI;
    public bool _player = false;
    public GameObject RobotText;

    private void Start()
    {
        _player = false;
        robotUI.SetActive(false);
        RobotText.SetActive(false);
    }

    private void Update()
    {
        // _player가 true일 때 Tab 키를 누르면 RobotUI 활성화/비활성화 전환
        if (_player && Input.GetKeyDown(KeyCode.Tab))
        {
            robotUI.SetActive(!robotUI.activeSelf);
        }

        if (_player)
        {
            RobotText.SetActive(true);
        }
        else
        {
            RobotText.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player = false;
            robotUI.SetActive(false);
        }
    }
}
