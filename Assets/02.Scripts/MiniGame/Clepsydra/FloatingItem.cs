using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FloatingItem : MonoBehaviour
{
    public float floatDuration = 2f; // 떠오르는 데 걸리는 시간
    public float floatHeight = 0.5f; // 떠오르는 높이

    // 회전
    public float rotationSpeed = 50f; // 회전 속도 (초당 각도)

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        Vector3 floatUpPosition = new Vector3(startPosition.x, startPosition.y + floatHeight, startPosition.z);
        transform.position = floatUpPosition;

        // 위아래로 부드럽게 움직이기
        transform.DOMoveY(startPosition.y, floatDuration)
            .SetLoops(-1, LoopType.Yoyo)  // 무한 반복(Yoyo: 올라갔다 내려왔다 반복)
            .SetEase(Ease.InOutSine);  // 부드러운 Sine 곡선을 사용하여 자연스러운 떠오름

        // 회전 애니메이션 설정
        transform.DORotate(new Vector3(0, 360, 0), rotationSpeed, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Incremental)  // 무한 반복 (회전 각도를 점진적으로 증가)
            .SetEase(Ease.Linear);  // 일정한 속도로 회전
    }
}
