using System.Collections;
using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    public Material daySkybox;  // 첫 번째 스카이박스(낮)
    public Material nightSkybox;  // 두 번째 스카이박스(밤)
    public float transitionDuration = 5.0f;  // 스카이박스 전환 시간

    private bool isTransitioning = false;

    // Skybox 전환 코루틴
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

        // 현재 스카이박스의 설정을 저장
        Material currentSkybox = RenderSettings.skybox;

        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            float t = timer / transitionDuration;

            // 스카이박스를 서서히 변경 (Blend)
            RenderSettings.skybox.Lerp(currentSkybox, nightSkybox, t);
            yield return null;
        }

        // 전환이 완료되었을 때 밤 스카이박스로 설정
        RenderSettings.skybox = nightSkybox;
        isTransitioning = false;
    }
}
