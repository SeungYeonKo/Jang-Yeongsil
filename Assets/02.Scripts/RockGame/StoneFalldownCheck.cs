using System.Collections;
using UnityEngine;

public class StoneFalldownCheck : MonoBehaviour
{
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private Rigidbody _rigidbody;
    private MeshRenderer _meshRenderer;
    private Collider _collider;

    private void Start()
    {
        // 비석의 원래 위치와 회전을 저장
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
        _rigidbody = GetComponent<Rigidbody>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<Collider>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        // 스톤이 충돌했는지 확인
        if (collision.gameObject.CompareTag("Stone"))
        {
            Debug.Log("비석 맞음");
            StartCoroutine(CheckPillarFall());
        }
    }

    private IEnumerator CheckPillarFall()
    {
        // 1초 대기 후 비석이 쓰러졌는지 확인
        yield return new WaitForSeconds(1.0f);
        // 비석이 일정 각도 이상 기울어졌다면 쓰러진 것으로 판단
        if (Mathf.Abs(transform.eulerAngles.x) > 30f || Mathf.Abs(transform.eulerAngles.z) > 30f)
        {
            // 비석 비활성화
            _meshRenderer.enabled = false;
            _collider.enabled = false;
            
            // 1초 후 비석을 다시 초기화하여 활성화
            yield return new WaitForSeconds(1.0f);
            // 위치에 랜덤 오프셋을 추가합니다.
            float randomOffsetX = Random.Range(-3f, 3f); // X축 랜덤 값
            float randomOffsetZ = Random.Range(-3f, 3f); // Z축 랜덤 값

            Vector3 newPosition = _originalPosition + new Vector3(randomOffsetX, 0, randomOffsetZ);

            transform.position = newPosition; // 새로운 랜덤 위치 설정
            transform.rotation = _originalRotation; // 원래 회전 값으로 초기화

            Debug.Log("비석이 원래 위치로 돌아감 (랜덤 오프셋 적용)");
            // 물리 상태 초기화
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            
            // 비석을 다시 활성화
            _meshRenderer.enabled = true;
            _collider.enabled = true;
        }
    }
}