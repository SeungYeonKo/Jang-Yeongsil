using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Countdown : MonoBehaviour
{
    public GameObject CountDownUI;
    public TextMeshProUGUI CountText;
    private GameState previousGameState;

    private UI_RainGaugeManager rainGaugeManager;

    private void Start()
    {
        previousGameState = RainGaugeManager.Instance.CurrentGameState;
        CountDownUI.SetActive(true); 
        CountText.gameObject.SetActive(false);
        rainGaugeManager = FindObjectOfType<UI_RainGaugeManager>();
    }

    private void Update()
    {
        GameState currentGameState = RainGaugeManager.Instance.CurrentGameState;

        if (currentGameState != previousGameState)
        {
            if (currentGameState == GameState.Loading)
            {
                StartCoroutine(ShowCountDown());
            }
            previousGameState = currentGameState;
        }
           
    }

    private IEnumerator ShowCountDown()
    {
        CountText.gameObject.SetActive(true);
        rainGaugeManager.ReadyImg.SetActive(false);

        for (int i = 5; i > 0; i--)
        {
            CountText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        CountText.gameObject.SetActive(false);
    }

}
