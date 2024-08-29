using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace _02.Scripts.RockGame
{
    public class StoneTimeAttack : MonoBehaviour
    {
        private StoneScoreManager _stoneScoreManager;
        private StoneObstacleController _obstacleController;
        private string _playerName;
        public float TimesUP = 60;
        public bool Isrunning = false;
        public bool IsBounsTimeStart = false;
        public bool IsWarnningStart = false;
        private void Start()
        {
            _stoneScoreManager = FindObjectOfType<StoneScoreManager>();
            _obstacleController = FindObjectOfType<StoneObstacleController>();
            _playerName = Photon.Pun.PhotonNetwork.NickName;
            Isrunning = true;
        }


        private void Update()
        {
            if (Isrunning)
            {
                if (TimesUP > 0)
                {
                    TimesUP -= Time.deltaTime;

                    // 10초 남았을 때 보너스 타임 및 경고 시작 여부를 확인
                    if (TimesUP <= 10f)
                    {
                        int currentScore = _stoneScoreManager.GetCurrentScore(_playerName);

                        if (currentScore < 50)
                        {
                            if (!IsWarnningStart) // 이미 경고가 시작되지 않았다면
                            {
                                IsWarnningStart = true;
                                _obstacleController.IsShowing = true;
                                _obstacleController._obstacle.SetActive(true);
                                _obstacleController.StartObstacle();
                                Debug.Log($"{_obstacleController.IsShowing}");
                            }
                        }
                        else
                        {
                            if (!IsBounsTimeStart) // 보너스 타임이 아직 시작되지 않았다면
                            {
                                IsBounsTimeStart = true;
                                Debug.Log("보너스 타임 시작! 점수가 100점씩 증가합니다.");
                            }
                        }
                    }
                }
                else
                {
                    // 시간이 다 되었을 때 처리
                    TimesUP = 0;
                    Isrunning = false;
                    IsWarnningStart = false;
                    IsBounsTimeStart = false;
                    Debug.Log("Time's up!");

                    // 최종 점수 제출
                    _stoneScoreManager.SubmitScore(_playerName);
                }
            }
        }

    }
}