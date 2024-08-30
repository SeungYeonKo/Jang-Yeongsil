using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstronomicalChartManager : MonoBehaviour
{
    private bool isCursorVisible = true;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SoundManager.instance.StopBgm();
        SoundManager.instance.PlayBgm(SoundManager.Bgm.AstronomicalChart);
    }

    void Update()
    {
        if (isCursorVisible)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void SetCursorVisibility(bool visible)
    {
        isCursorVisible = visible;
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
