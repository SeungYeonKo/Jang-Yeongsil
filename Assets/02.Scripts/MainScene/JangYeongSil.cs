using TMPro;
using UnityEngine;

public class JangYeongSil : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public float ChangeInterval = 2f;

    private Vector3 targetPosition;

    public GameObject JangYeongSilMent;
    public TextMeshProUGUI JangYeongSilMentText;
    public string[] mentArray;
    public float minMentInterval = 20f;
    public float maxMentInterval = 40f;

    // 이동 가능 범위
    public BoxCollider movementArea;

    void Start()
    {
        SetRandomTargetPosition();
        // 방향변경 코루틴
        StartCoroutine(ChangeDirectionPeriodically());
        // 멘트 코루틴
        StartCoroutine(ChangeMentPeriodically());
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
            JangYeongSilMentText.text = mentArray[randomIndex];

            // 말풍선 활성화
            JangYeongSilMent.SetActive(true);

            // 일정 시간 후 말풍선 비활성화
            StartCoroutine(HideMentAfterDelay(5f)); // 5초 후 비활성화
        }
    }

    System.Collections.IEnumerator HideMentAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        JangYeongSilMent.SetActive(false);
    }
}
