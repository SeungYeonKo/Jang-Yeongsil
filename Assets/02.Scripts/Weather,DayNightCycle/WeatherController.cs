using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    public class WeatherController : MonoBehaviourPun
    {
        public WeatherMakerPrecipitationProfileScript RainProfile;      // 비 
        public WeatherMakerPrecipitationProfileScript NoneProfile;      // 맑음

        private WeatherMakerPrecipitationManagerScript precipitationManager;

        void Start()
        {
            precipitationManager = GetComponent<WeatherMakerPrecipitationManagerScript>();
            if (precipitationManager == null)
            {
                Debug.LogError("WeatherMakerPrecipitationManagerScript is not found.");
                return;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(DailyWeatherRoutine());
            }
        }

        private IEnumerator DailyWeatherRoutine()
        {
            yield return new WaitForSeconds(60f); // 1분마다 날씨 변경

            while (true)
            {
                SetDailyWeather();
                yield return new WaitForSeconds(60f); // 1분마다 날씨 변경

            }
        }

        private void SetDailyWeather()
        {
            float randomValue = Random.Range(0f, 1f);
            int weatherType = 0;

            if (randomValue < 0.2f)
            {
                SetWeather(RainProfile);
                weatherType = 1;
            }
            else
            {
                SetWeather(NoneProfile);
                weatherType = 0;
            }
            photonView.RPC("SyncWeather", RpcTarget.Others, weatherType);
        }

        [PunRPC]
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
            //precipitationManager.SetPrecipitationProfile(profile);
        }
    }
}
