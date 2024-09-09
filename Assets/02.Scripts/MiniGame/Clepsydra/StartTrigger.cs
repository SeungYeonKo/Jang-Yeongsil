using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 씬 변경 이벤트를 위해 추가

public class StartTrigger : MonoBehaviour
{
    public Image StartImage;
    public Button StartButton;
    public Transform Maze1StartPosition;

    public GameObject ItemSlot;

    public bool isMazeStart;

    private GameObject player;

    private void Start()
    {
        StartButton.onClick.AddListener(OnStartButtonClick);
        isMazeStart = false;
        ItemSlot.SetActive(false);

        // 씬 변경 시 BGM을 페이드 아웃시키기 위한 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // 씬 변경 시 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (isMazeStart && !ItemSlot.activeSelf)
        {
            ItemSlot.SetActive(true);
        }
    }

    public void OnStartButtonClick()
    {
        Debug.Log("미로 입장하기 버튼 클릭");

        if (player != null && Maze1StartPosition != null)
        {
            player.transform.position = Maze1StartPosition.position;
            isMazeStart = true;

            // 미로 시작할 때 ClepsydraScene 배경음악 재생
            SoundManager.instance.PlayBgm(SoundManager.Bgm.ClepsydraScene);
        }

        if (StartImage != null)
        {
            StartImage.gameObject.SetActive(false);
        }

        if (StartButton != null)
        {
            StartButton.gameObject.SetActive(false);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (StartImage != null)
            {
                StartImage.gameObject.SetActive(true);
            }

            if (StartButton != null)
            {
                StartButton.gameObject.SetActive(true);
            }

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (StartImage != null)
            {
                StartImage.gameObject.SetActive(false);
            }

            if (StartButton != null)
            {
                StartButton.gameObject.SetActive(false);
            }

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            player = null;
        }
    }

    // 씬이 로드될 때 호출
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 변경되면 배경음악 페이드 아웃 실행
        SoundManager.instance.FadeOutBgm(2.3f);
    }
}
