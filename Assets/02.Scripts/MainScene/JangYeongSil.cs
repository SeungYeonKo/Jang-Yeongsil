using TMPro;
using UnityEngine;

public class JangYeongSil : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public float ChangeInterval = 2f;

    private Vector3 targetPosition;

    public GameObject speechBubblePrefab; 
    private GameObject speechBubbleInstance; 
    private TextMeshProUGUI speechBubbleText;

    public string[] mentArray;
    public float minMentInterval = 20f;
    public float maxMentInterval = 40f;

    // 이동 가능 범위
    public BoxCollider movementArea;

    void Start()
    {
        SetRandomTargetPosition();
        // 방향 변경 코루틴
        StartCoroutine(ChangeDirectionPeriodically());
        // 멘트 변경 코루틴
        StartCoroutine(ChangeMentPeriodically());

        // 말풍선 인스턴스 생성
        speechBubbleInstance = Instantiate(speechBubblePrefab, transform);
        speechBubbleText = speechBubbleInstance.GetComponentInChildren<TextMeshProUGUI>();

        // 말풍선 위치를 게임 오브젝트의 머리 위로 설정
        speechBubbleInstance.transform.localPosition = new Vector3(0, 0, 0); // 오브젝트 머리 위로 말풍선 위치 설정
    }

    void Update()
    {
        // 현재 위치에서 타겟 위치로 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);

        // 타겟 위치에 도달하면 새로운 타겟 위치 설정
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetRandomTargetPosition();
        }

        // 말풍선을 카메라 방향으로 회전
        speechBubbleInstance.transform.LookAt(Camera.main.transform);
        speechBubbleInstance.transform.Rotate(0, 180, 0); // 텍스트가 뒤집히지 않도록 180도 회전
    }

    void SetRandomTargetPosition()
    {
        if (movementArea != null)
        {
            // 박스 콜라이더의 범위 내에서 랜덤 위치 설정
            Vector3 center = movementArea.bounds.center;
            Vector3 size = movementArea.bounds.size;
            float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
            float y = transform.position.y; // Y축 고정
            float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);
            targetPosition = new Vector3(x, y, z);
        }
        else
        {
            Debug.LogError("Movement area (BoxCollider) is not assigned.");
        }
    }

    System.Collections.IEnumerator ChangeDirectionPeriodically()
    {
        while (true)
        {
            // 주기적으로 새로운 타겟 위치 설정
            yield return new WaitForSeconds(ChangeInterval);
            SetRandomTargetPosition();
        }
    }

    System.Collections.IEnumerator ChangeMentPeriodically()
    {
        while (true)
        {
            // 랜덤 멘트 변경 간격 설정
            float interval = Random.Range(minMentInterval, maxMentInterval);
            yield return new WaitForSeconds(interval);
            SetRandomMent();
        }
    }

    void SetRandomMent()
    {
        if (mentArray.Length > 0)
        {
            // 랜덤으로 멘트 선택
            int randomIndex = Random.Range(0, mentArray.Length);
            speechBubbleText.text = mentArray[randomIndex];

            // 말풍선 활성화
            speechBubbleInstance.SetActive(true);

            // 일정 시간 후 말풍선 비활성화
            StartCoroutine(HideMentAfterDelay(5f)); // 5초 후 비활성화
        }
    }

    System.Collections.IEnumerator HideMentAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        speechBubbleInstance.SetActive(false);
    }
}
