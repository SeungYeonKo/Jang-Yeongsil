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
        Debug.Log("시간이 지남");
        // 비석이 일정 각도 이상 기울어졌다면 쓰러진 것으로 판단
        if (Mathf.Abs(transform.eulerAngles.x) > 30f || Mathf.Abs(transform.eulerAngles.z) > 30f)
        {
            // 비석 비활성화
            _meshRenderer.enabled = false;
            _collider.enabled = false;

            Debug.Log("비석이 숨겨짐");
            // 1초 후 비석을 다시 초기화하여 활성화
            yield return new WaitForSeconds(1.0f);
            Debug.Log("시간이 지남");
            // 위치와 회전 값 초기화
            transform.position = _originalPosition;
            transform.rotation = _originalRotation;
            Debug.Log("상태 초기화");
            // 물리 상태 초기화
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            
            // 비석을 다시 활성화
            _meshRenderer.enabled = true;
            _collider.enabled = true;
        }
    }
}