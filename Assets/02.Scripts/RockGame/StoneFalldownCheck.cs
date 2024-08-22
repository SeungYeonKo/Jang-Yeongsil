using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneFalldownCheck : MonoBehaviour
{
    private Transform _oriTransform;

    private void Start()
    {
        _oriTransform.position = transform.position;
    }

    IEnumerator CheckPillarFall(Rigidbody pillarRb, GameObject pillar)
    {
        yield return new WaitForSeconds(1.0f);

        if (Mathf.Abs(pillarRb.transform.eulerAngles.x) > 30f || Mathf.Abs(pillarRb.transform.eulerAngles.z) > 30f)
        {
            // 비석 비활성화
            pillar.SetActive(false);

            // 1초 후 비석을 다시 초기화하여 활성화
            yield return new WaitForSeconds(1.0f);

            // 위치와 회전 값 초기화
            pillar.transform.position = _oriTransform.transform.position; // 기존 위치
            pillar.transform.rotation = Quaternion.identity; // 초기 회전 값 (수직으로 설정)

            // 물리 상태 초기화
            pillarRb.velocity = Vector3.zero;
            pillarRb.angularVelocity = Vector3.zero;

            // 비석을 다시 활성화
            pillar.SetActive(true);
        }
    }
}
