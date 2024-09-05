using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace _02.Scripts.Scene
{
    public class LoadingScene : MonoBehaviourPunCallbacks
    {
        public Slider loadingSlider;
        public TextMeshProUGUI progressText;
        private float _timer = 0f;
        private bool isRoomJoined = false;  // 방 입장 상태 확인
        private bool isLoadingComplete = false;

        private void Start()
        {
            // 다음 씬을 비동기로 로드하는 코루틴 실행
            StartCoroutine(LoadSceneAsync());
            SoundManager.instance.StopBgm();
            SoundManager.instance.PlayBgm(SoundManager.Bgm.Loading);
        }

        IEnumerator LoadSceneAsync()
        {
            while (!isLoadingComplete) // 로딩이 완료될 때까지 진행
            {
                // 타이머가 더 빠르게 증가하도록 설정 (2배 빠르게)
                _timer += Time.deltaTime * 3;

                // 슬라이더 값 업데이트 (3초 안에 100% 도달하도록 조정)
                loadingSlider.value = _timer / 3f;

                // 퍼센트 값 계산 (0에서 100까지)
                int progressPercentage = Mathf.RoundToInt(loadingSlider.value * 100);

                // progressText에 퍼센트 값 설정
                progressText.text = progressPercentage + "%";

                // 만약 슬라이더가 100%에 도달하면 로딩 완료 플래그 설정
                if (loadingSlider.value >= 1f)
                {
                    loadingSlider.value = 1f;
                    progressText.text = "100%";
                    isLoadingComplete = true;
                }
                yield return null;  // 매 프레임마다 업데이트
            }
        }

        // Photon 방에 성공적으로 입장했을 때 콜백 함수
        public override void OnJoinedRoom()
        {
            Debug.Log("방에 성공적으로 입장했습니다.");
            isRoomJoined = true;  // 방 입장 완료 플래그 설정
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("방 입장 실패: " + message);
        }
    }
}