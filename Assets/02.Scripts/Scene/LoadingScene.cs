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
        private float _timer;
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
                _timer += Time.deltaTime;

                loadingSlider.value = _timer / 10f;
                // 퍼센트 값 계산 (0에서 100까지)
                int progressPercentage = Mathf.RoundToInt(loadingSlider.value * 100);

                // progressText에 퍼센트 값 설정
                progressText.text = progressPercentage + "%";

                if (_timer > 10)
                {
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }
    }
}