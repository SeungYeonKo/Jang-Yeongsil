using System.Collections;
using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    public Material daySkybox;  // 낮 스카이박스
    public Material nightSkybox;  // 밤 스카이박스
    public float transitionDuration = 2f;  // 스카이박스 전환 시간

    private bool isTransitioning = false;

    // 시작 시에 어두운 하늘 설정
    private void Start()
    {
        // 처음엔 밤 스카이박스를 설정
        RenderSettings.skybox = nightSkybox;
    }

    // 3문제를 맞춘 후 호출할 스카이박스 전환 함수
    public void StartSkyboxTransition()
    {
        if (!isTransitioning)
        {
            StartCoroutine(SkyboxTransitionCoroutine());
        }
    }

    // 스카이박스를 낮하늘로 전환하는 코루틴
    private IEnumerator SkyboxTransitionCoroutine()
    {
        isTransitioning = true;
        float timer = 0.0f;

        // 현재 노출도를 저장 (초기값은 1.0, 어두운 밤)
        float initialExposure = RenderSettings.skybox.GetFloat("_Exposure");
        float targetExposure = 1.3f;  // 전환할 목표 노출도 (밝은 하늘)

        // 스카이박스를 현재 밤 스카이박스로 설정
        RenderSettings.skybox = nightSkybox;

        // transitionDuration 동안 서서히 밝아지는 효과
        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            float t = timer / transitionDuration;

            // 스카이박스의 노출도를 서서히 높여 밝아지게 함
            RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(initialExposure, targetExposure, t));

            yield return null;
        }

        // 전환 완료 후 낮 스카이박스로 변경
        RenderSettings.skybox = daySkybox;
        RenderSettings.skybox.SetFloat("_Exposure", 1.3f);  // 낮 스카이박스 노출도 설정

        isTransitioning = false;
    }
}
