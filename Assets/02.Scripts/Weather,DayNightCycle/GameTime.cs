using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    public enum TimeType
    {
        Day,
        Night,
    }

    public TimeType CurrentTimeType;
    public delegate void TimeTypeChangedHandler(TimeType newTimeType);


    void Start()
    {
        CurrentTimeType = TimeType.Day;
    }
}
