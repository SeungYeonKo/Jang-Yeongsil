using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    public GameObject Description;
    public GameObject Video;
    public PlayableDirector playableDirector;
    public TextMeshProUGUI IntroText;

    void Start()
    {
        IntroText.gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Description.SetActive(false);
        Video.SetActive(false);
    }
    
    public void ShowDescription()
    {
        IntroText.gameObject.SetActive(false);
        Video.SetActive(false);
        Description.SetActive(true);
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }

    public void ShowVideo()
    {
        IntroText.gameObject.SetActive(false);
        Description.SetActive(false);
        Video.SetActive(true);
        playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0.5f);
        playableDirector.Play();
    }
}
