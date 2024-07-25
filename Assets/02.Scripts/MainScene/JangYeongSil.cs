using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    public float MoveSpeed = 5f;  
    public float ChangeInterval = 2f;  

    private Vector3 targetPosition;

    void Start()
    {
        // 초기 타겟 위치 설정
        SetRandomTargetPosition();
        // 주기적으로 방향을 변경하는 코루틴 시작
        StartCoroutine(ChangeDirectionPeriodically());
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
        // 새로운 랜덤 위치 설정
        float x = Random.Range(-10f, 10f);
        float y = Random.Range(-10f, 10f);
        float z = Random.Range(-10f, 10f);
        targetPosition = new Vector3(x, y, z);
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
}
