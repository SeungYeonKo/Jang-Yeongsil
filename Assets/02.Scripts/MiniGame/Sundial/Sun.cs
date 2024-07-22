//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// 소스 코드는 개인 또는 상업 프로젝트에서 사용할 수 있습니다.
// 소스 코드는 재배포 또는 판매할 수 없습니다.
// 
// *** 불법 복제에 대한 메모 ***
// 
// 이 자산을 해적 사이트에서 얻었다면, Unity 자산 스토어에서 구매를 고려해 주세요. https://assetstore.unity.com/packages/slug/60955?aid=1011lGnL 이 자산은 Unity 자산 스토어에서만 법적으로 사용할 수 있습니다.
// 
// 저는 수백, 수천 시간을 들여 이 자산과 다른 자산을 개발하여 가족을 부양하는 인디 개발자입니다. 이렇게 열심히 작업한 소프트웨어를 도둑맞는 것은 매우 모욕적이고, 무례하며, 단순히 악의적인 행위입니다.
// 
// 감사합니다.
//
// *** 불법 복제에 대한 메모 종료 ***
//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DigitalRuby.WeatherMaker
{
    /// <summary>
    /// 낮/밤 주기 관리자
    /// </summary>
    [ExecuteInEditMode]
    public class Sun : MonoBehaviour
    {
        /// <summary>낮/밤 주기 프로필 및 색상 체계</summary>
        [Tooltip("낮/밤 주기 프로필 및 색상 체계")]
        public WeatherMakerDayNightCycleProfileScript DayNightProfile;

#if UNITY_EDITOR

#pragma warning disable 0414

        [ReadOnlyLabel]
        [SerializeField]
        private string TimeOfDayLabel = string.Empty;

#pragma warning restore 0414

#endif

        /// <summary>
        /// 낮 속도
        /// </summary>
        public float Speed { get { return DayNightProfile.Speed; } set { DayNightProfile.Speed = value; } }

        /// <summary>
        /// 밤 속도
        /// </summary>
        public float NightSpeed { get { return DayNightProfile.NightSpeed; } set { DayNightProfile.NightSpeed = value; } }

        /// <summary>
        /// 하루 시간 (초), 0에서 86400
        /// </summary>
        public float TimeOfDay { get { return DayNightProfile.TimeOfDay; } set { DayNightProfile.TimeOfDay = value; } }

        /// <summary>
        /// 하루 시간 카테고리
        /// </summary>
        public WeatherMakerTimeOfDayCategory TimeOfDayCategory { get { return DayNightProfile.TimeOfDayCategory; } }

        /// <summary>
        /// 하루 시간을 TimeSpan 객체로 가져옴
        /// </summary>
        public TimeSpan TimeOfDayTimespan { get { return DayNightProfile.TimeOfDayTimespan; } set { DayNightProfile.TimeOfDayTimeSpan = value; } }

        /// <summary>
        /// 시간대 오프셋 (초)
        /// </summary>
        public int TimeZoneOffsetSeconds { get { return DayNightProfile.TimeZoneOffsetSeconds; } set { DayNightProfile.TimeZoneOffsetSeconds = value; } }

        /// <summary>
        /// 연도
        /// </summary>
        public int Year {  get { return DayNightProfile.Year; } set { DayNightProfile.Year = value; } }

        /// <summary>
        /// 월
        /// </summary>
        public int Month { get { return DayNightProfile.Month; } set { DayNightProfile.Month = value; } }

        /// <summary>
        /// 일
        /// </summary>
        public int Day { get { return DayNightProfile.Day; } set { DayNightProfile.Day = value; } }

        /// <summary>
        /// 현재 연도, 월, 일 및 하루 시간을 나타내는 DateTime 객체 가져오기
        /// </summary>
        public DateTime DateTime
        {
            get { return DayNightProfile.DateTime; }
            set { DayNightProfile.DateTime = value; }
        }

        /// <summary>
        /// 위도 (도)
        /// </summary>
        public double Latitude { get { return DayNightProfile.Latitude; } set { DayNightProfile.Latitude = value; } }

        /// <summary>
        /// 경도 (도)
        /// </summary>
        public double Longitude { get { return DayNightProfile.Longitude; } set { DayNightProfile.Longitude = value; } }

        /// <summary>
        /// 완전히 낮인 경우 1
        /// </summary>
        public float DayMultiplier { get { return DayNightProfile.DayMultiplier; } }

        /// <summary>
        /// 완전히 새벽 또는 황혼인 경우 1
        /// </summary>
        public float DawnDuskMultiplier { get { return DayNightProfile.DawnDuskMultiplier; } }

        /// <summary>
        /// 완전히 밤인 경우 1
        /// </summary>
        public float NightMultiplier { get { return DayNightProfile.NightMultiplier; } }

        /// <summary>
        /// 태양 데이터
        /// </summary>
        public WeatherMakerDayNightCycleProfileScript.SunInfo SunData { get { return DayNightProfile.SunData; } }

        private void EnsureProfile()
        {
            if (WeatherMakerScript.Instance == null)
            {
                return;
            }

            if (DayNightProfile == null)
            {
                DayNightProfile = WeatherMakerScript.Instance.LoadResource<WeatherMakerDayNightCycleProfileScript>("WeatherMakerDayNightCycleProfile_Default");
            }

            if (Application.isPlaying)
            {
                DayNightProfile = ScriptableObject.Instantiate(DayNightProfile);
            }
        }

        private void OnEnable()
        {
            WeatherMakerScript.EnsureInstance(this, ref instance);
        }

        private void Awake()
        {
            EnsureProfile();
        }

        private void Start()
        {
            EnsureProfile();
            DayNightProfile.UpdateFromProfile(WeatherMakerScript.Instance != null && WeatherMakerScript.Instance.NetworkConnection.IsServer);
        }

        private void Update()
        {
            DayNightProfile.UpdateFromProfile(WeatherMakerScript.Instance != null && WeatherMakerScript.Instance.NetworkConnection.IsServer);

#if UNITY_EDITOR

            TimeOfDayLabel = DayNightProfile.TimeOfDayLabel;

#endif

        }

        private void OnDestroy()
        {
            WeatherMakerScript.ReleaseInstance(ref instance);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitOnLoad()
        {
            WeatherMakerScript.ReleaseInstance(ref instance);
        }

        private static WeatherMakerDayNightCycleManagerScript instance;
        /// <summary>
        /// 낮/밤 주기 관리자 스크립트의 공유 인스턴스
        /// </summary>
        public static WeatherMakerDayNightCycleManagerScript Instance
        {
            get { return WeatherMakerScript.FindOrCreateInstance(ref instance, true); }
        }

        /// <summary>
        /// 낮/밤 관리자 인스턴스가 있는지 확인
        /// </summary>
        /// <returns>인스턴스가 있으면 true, 그렇지 않으면 false</returns>
        public static bool HasInstance()
        {
            return instance != null;
        }
    }
}

// 참고자료:
// https://en.wikipedia.org/wiki/Position_of_the_Sun
// http://stackoverflow.com/questions/8708048/position-of-the-sun-given-time-of-day-latitude-and-longitude
// http://www.grasshopper3d.com/forum/topics/solar-calculation-plugin
// http://guideving.blogspot.nl/2010/08/sun-position-in-c.html
// https://github.com/mourner/suncalc
// http://stackoverflow.com/questions/1058342/rough-estimate-of-the-time-offset-from-gmt-from-latitude-longitude
// http://www.stjarnhimlen.se/comp/tutorial.html
// http://www.suncalc.net/#/40.7608,-111.891,12/2000.09.21/12:46
// http://www.suncalc.net/scripts/suncalc.js

// 개기 일식:
// 43.7678
// -111.8323
// 최대 일식: 2017/08/21 17:34:18.6 49.5° 133.1°
