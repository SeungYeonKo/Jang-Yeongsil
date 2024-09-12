using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using TMPro;
using UnityEngine;

public class JangChatChoice : MonoBehaviour
{
    public GameObject Choices;
    public GameObject TextBG;
    public List<TextMeshProUGUI> Texts;
    public GameObject FindMe;
    public GameObject StoryMode;
    public StoryTrigger StoryTrigger;
    void Start()
    {
        FindMe.SetActive(false);
        StoryMode.SetActive(false);
        Choices.SetActive(false);
        TextBG.SetActive(false);
        StartCoroutine(Show_coroutine());
        foreach (var textMesh in Texts)
        {
            textMesh.gameObject.SetActive(false);
        }
    }

    IEnumerator Show_coroutine()
    {
        FindMe.SetActive(true);
        yield return new WaitForSeconds(2f);
        FindMe.SetActive(false);
    }

    public void TriggerStart()
    {
        Cursor.lockState = CursorLockMode.None; // 마우스 커서 잠금 해제
        Cursor.visible = true;                  // 마우스 커서 보이기
        FindMe.SetActive(false);
        Choices.SetActive(true);
        TextBG.SetActive(false);
        foreach (var textMesh in Texts)
        {
            textMesh.gameObject.SetActive(false);
        }
    }
    
    public void OnClickSunButton()
    {
        Choices.SetActive(false);
        TextBG.SetActive(true);
        Texts[0].gameObject.SetActive(true);
    }
    public void OnClickRainButton()
    {
        Choices.SetActive(false);
        TextBG.SetActive(true);
        Texts[1].gameObject.SetActive(true);
    }
    public void OnClickClepsyButton()
    {
        Choices.SetActive(false);
        TextBG.SetActive(true);
        Texts[2].gameObject.SetActive(true);
    }
    public void OnClickSkyButton()
    {
        Choices.SetActive(false);
        TextBG.SetActive(true);
        Texts[3].gameObject.SetActive(true);
    }

    public void OnClickStoryModeButton()
    {
        Choices.SetActive(false);
        TextBG.SetActive(false);
        foreach (var textMesh in Texts)
        {
            textMesh.gameObject.SetActive(false);
        }
        StoryTrigger.StartStory();
        StoryMode.SetActive(true);
    }

    public void OnClickRetryButton()
    {
        Choices.SetActive(true);
        TextBG.SetActive(false);
        foreach (var textMesh in Texts)
        {
            textMesh.gameObject.SetActive(false);
        }
    }

    public void OnClickXbutton()
    {
        FindMe.SetActive(false);
        Choices.SetActive(false);
        TextBG.SetActive(false);
        foreach (var textMesh in Texts)
        {
            textMesh.gameObject.SetActive(false);
        }
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }
}