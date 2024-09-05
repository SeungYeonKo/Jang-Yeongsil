using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _02.Scripts.Scene
{
    public class LoadingScene : MonoBehaviour
    {
        public Slider loadingSlider;
        public TextMeshProUGUI progressText;

        private void Start()
        {
            // 다음 씬을 비동기로 로드하는 코루틴 실행
            StartCoroutine(LoadSceneAsync());
        }

        IEnumerator LoadSceneAsync()
        {
            // 메인 씬을 비동기로 로드
            AsyncOperation operation = SceneManager.LoadSceneAsync("MainScene");

            // 로딩이 완료되기 전까지는 씬을 자동으로 활성화하지 않음
            operation.allowSceneActivation = false;

            // 로딩이 진행되는 동안 슬라이더와 텍스트 업데이트
            while (!operation.isDone)
            {
                // operation.progress는 0에서 0.9까지 진행됨 (1은 씬 활성화 시점)
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                loadingSlider.value = progress;
                progressText.text = Mathf.RoundToInt(progress * 100f) + "%";

                // 씬 로드가 완료되었을 때
                if (operation.progress >= 0.9f)
                {
                    // 로딩이 끝나면 씬을 활성화 (슬라이더가 끝에 도달하면)
                    loadingSlider.value = 1f;
                    progressText.text = "100%";
                
                    // 씬을 활성화
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }
    }
}