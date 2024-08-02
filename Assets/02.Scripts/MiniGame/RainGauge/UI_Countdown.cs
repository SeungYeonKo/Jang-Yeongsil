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

    private void Start()
    {
        previousGameState = RainGaugeManager.Instance.CurrentGameState;
        CountDownUI.SetActive(false);
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
        CountDownUI.SetActive(true);
        for (int i = 5; i > 0; i--)
        {
            CountText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        CountDownUI.SetActive(false);
    }

}
