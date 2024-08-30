using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
  public delegate void DelegateOnClick();
  public DelegateOnClick btnPlayOnClick;

  public GameObject panelTopPanel;
  public GameObject panelBottomPanel;
  public GameObject panelGameCompletion;
  public GameObject panelGameMode;

  public Text textTime;
  public Text textTotalTiles;
  public Text textTilesInPlace;
  public Text textGameCompletionMessage;

  public BoardGen boardGen;

  IEnumerator FadeInUI(GameObject panel, float fadeInDuration = 2.0f)
  {
    Graphic[] graphics = panel.GetComponentsInChildren<Graphic>();
    foreach(Graphic graphic in graphics)
    {
      graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 0.0f);
    }

    float timer = 0.0f;
    while(timer < fadeInDuration)
    {
      timer += Time.deltaTime;
      float normalisedTime = timer / fadeInDuration;
      foreach(Graphic graphic in graphics)
      {
        graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, normalisedTime);
      }
      yield return null;
    }
    foreach (Graphic graphic in graphics)
    {
      graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 1.0f);
    }
  }

  public void SetEnableBottomPanel(bool flag)
  {
    panelBottomPanel.SetActive(flag);
    if (flag)
    {
      FadeInUI(panelBottomPanel);
    }
  }

  public void SetEnableTopPanel(bool flag)
  {
    panelTopPanel.SetActive(flag);
    if (flag)
    {
      FadeInUI(panelTopPanel);
    }
  }

  public void OnClickPlay()
  {
    btnPlayOnClick?.Invoke();
  }

  public void SetTimeInSeconds(double tt)
  {
    System.TimeSpan t = System.TimeSpan.FromSeconds(tt);
    string time = string.Format("{0:D2} : {1:D2} : {2:D2}", t.Hours, t.Minutes, t.Seconds);

    textTime.text = time;
  }

  public void SetTotalTiles(int count)
  {
    textTotalTiles.text = count.ToString();
  }

  public void SetTilesInPlace(int count)
  {
    textTilesInPlace.text = count.ToString();
  }

  public void SetEnableGameCompletionPanel(bool flag)
  {
    panelGameCompletion.SetActive(flag);
    if(flag)
    {
      FadeInUI(panelGameCompletion);
      UpdateGameCompletionMessage();
    }
  }

    public void SetEnablePanelGameMode(bool flag)
    {
        panelGameMode.SetActive(flag);
        if (flag)
        {
            FadeInUI(panelGameMode);
        }
    }

  public void OnClickExit()
  {
        PhotonManager.Instance.LeaveAndLoadRoom("Main");
  }

  public void OnClickPlayAgain()
  {
    SceneManager.LoadScene("AstronomicalChartScene");
  }
    public void OnClickHomeScreen()
    {
        if (boardGen != null)
        {
            boardGen.CompletePuzzle(false);
        }
        PartialOnPuzzleCompleted(); 
        SceneManager.LoadScene("AstronomicalChartScene");
    }
    public void OnClickEasyMode()
    {
        GameApp.Instance.SetMode("Easy");
        SceneManager.LoadScene("AstronomicalChartScene");
    }

    public void OnClickNormalMode()
    {
        GameApp.Instance.SetMode("Normal");
        SceneManager.LoadScene("AstronomicalChartScene");
    }

    public void OnClickHardMode()
    {
        GameApp.Instance.SetMode("Hard");
        SceneManager.LoadScene("AstronomicalChartScene");
    }

    private void UpdateGameCompletionMessage()
    {
        string mode = GameApp.Instance.GetCurrentMode();

        switch (mode)
        {
            case "Easy":
                textGameCompletionMessage.text = "축하합니다! 견우성과 직녀성 퍼즐을 완성하셨습니다.";
                break;
            case "Normal":
                textGameCompletionMessage.text = "축하합니다! 천문도 퍼즐을 완성하셨습니다.";
                break;
            case "Hard":
                textGameCompletionMessage.text = "축하합니다! 북두칠성과 북극성 퍼즐을 완성하셨습니다.";
                break;
            default:
                textGameCompletionMessage.text = "축하합니다! 퍼즐을 완성하셨습니다.";
                break;
        }
    }

    private void PartialOnPuzzleCompleted()
    {
        SetEnableTopPanel(false);
        GameApp.Instance.SecondsSinceStart = 0;
        GameApp.Instance.TotalTilesInCorrectPosition = 0;
    }
}
