using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Countdown : MonoBehaviour
{
    public GameObject CountDownUI;
    public TextMeshProUGUI CountText;
    public Image RoadingImage;

    private void Start()
    {
        CountDownUI.SetActive(false);
    }

    private void Update()
    {
        if (RainGaugeManager.Instance.CurrentGameState == GameState.Loading)
        {
            StartCoroutine(ShowCountDown());
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
