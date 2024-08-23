using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace _02.Scripts.RockGame
{
    public class StoneTimeAttack : MonoBehaviour
    {
        private StoneScoreManager _stoneScoreManager;
        private string _playerName;
        public float TimesUP = 60;
        public bool Isrunning = false;

        private void Start()
        {
            _stoneScoreManager = FindObjectOfType<StoneScoreManager>();
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
                    // 10초 남았을 때 현재 점수를 확인합니다.
                    if (TimesUP <= 10f)
                    {
                        int currentScore = _stoneScoreManager.GetCurrentScore(_playerName);

                        if (currentScore < 50)
                        {
                            // 점수가 낮을 때 장애물 등장 로직
                            Debug.Log("장애물이 등장합니다!");
                            // 장애물을 활성화하는 로직을 추가합니다.
                        }
                        else
                        {
                            // 점수가 높을 때 점수 증가율을 변경합니다.
                            Debug.Log("점수가 높아졌습니다! 30점에서 100점으로 변경.");
                            // 이후부터 점수가 100점씩 올라가도록 설정
                            _stoneScoreManager.AddScoreForPlayer(_playerName, 100); // 예시: 100점으로 변경
                        }
                    }
                }
                else
                {
                    TimesUP = 0;
                    Isrunning = false;
                    Debug.Log("Time's up!");
                    _stoneScoreManager.SubmitScore(_playerName);
                }
            }
        }
    }
}