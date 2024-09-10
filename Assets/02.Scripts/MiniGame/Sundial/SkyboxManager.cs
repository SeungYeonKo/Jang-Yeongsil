using System.Collections;
using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    public Camera nightCamera;  // 밤하늘 스카이박스를 렌더링할 카메라
    public Camera dayCamera;    // 낮하늘 스카이박스를 렌더링할 카메라
    public float transitionDuration = 5.0f;  // 전환 시간

    private bool isTransitioning = false;

    private void Start()
    {
        // 시작 시 밤 카메라만 활성화
        nightCamera.enabled = true;
        dayCamera.enabled = false;
    }

    public void StartSkyboxTransition()
    {
        if (!isTransitioning)
        {
            StartCoroutine(SkyboxTransitionCoroutine());
        }
    }

    private IEnumerator SkyboxTransitionCoroutine()
    {
        isTransitioning = true;
        float timer = 0.0f;

        // 낮 카메라 활성화
        dayCamera.enabled = true;

        // 카메라 블렌딩 비율을 서서히 조정
        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            float blend = Mathf.Clamp01(timer / transitionDuration);

            // 밤 카메라와 낮 카메라의 블렌딩 비율을 조정 (렌더링 비율)
            nightCamera.rect = new Rect(0, 0, 1 - blend, 1);
            dayCamera.rect = new Rect(blend, 0, 1, 1);

            yield return null;
        }

        // 전환 완료 후 밤 카메라 비활성화
        nightCamera.enabled = false;
        isTransitioning = false;
    }
}
