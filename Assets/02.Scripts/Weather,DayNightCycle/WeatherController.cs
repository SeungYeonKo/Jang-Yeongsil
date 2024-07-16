using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DigitalRuby.WeatherMaker
{
    public class WeatherController : MonoBehaviourPun
    {
        public WeatherMakerPrecipitationProfileScript RainProfile;      // 비 
        public WeatherMakerPrecipitationProfileScript NoneProfile;      // 맑음

        private WeatherMakerPrecipitationManagerScript precipitationManager;

        void Start()
        {
            precipitationManager = FindObjectOfType<WeatherMakerPrecipitationManagerScript>();
            if (precipitationManager == null)
            {
                Debug.LogError("WeatherMakerPrecipitationManagerScript is not found.");
                return;
            }

            if (SceneManager.GetActiveScene().name == "RainGauge")
            {
                SetWeather(RainProfile);
                Debug.Log("현재 씬이 'RainGauge'입니다. 비가 내립니다.");
            }
            //if (PhotonNetwork.IsMasterClient)
            else
            {
                StartCoroutine(DailyWeatherRoutine());
            }
        }

        private IEnumerator DailyWeatherRoutine()
        {
            Debug.Log("날씨 랜덤 돌리기 시작");
            yield return new WaitForSeconds(10f); // n초마다 날씨 변경

            while (true)
            {
                SetDailyWeather();
                yield return new WaitForSeconds(10f);   // n초마다 날씨 변경

            }
        }

        private void SetDailyWeather()
        {
            int randomValue = Random.Range(0, 2); // 0 또는 1의 값을 생성
            int weatherType = 0;

            if (randomValue == 0)
            {
                SetWeather(RainProfile);
                weatherType = 1;
                Debug.Log("날씨 랜덤 : 비");
            }
            else
            {
                SetWeather(NoneProfile);
                weatherType = 0;
                Debug.Log("날씨 랜덤 : 맑음");
            }
            //photonView.RPC("SyncWeather", RpcTarget.Others, weatherType);
        }

       // [PunRPC]
        private void SyncWeather(int weatherType)
        {
            switch (weatherType)
            {
                case 1:
                    SetWeather(RainProfile);
                    break;
                default:
                    SetWeather(NoneProfile);
                    break;
            }
        }

        private void SetWeather(WeatherMakerPrecipitationProfileScript profile)
        {
            precipitationManager.SetPrecipitationProfile(profile);
        }
    }
}
