using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CartoonIntro : MonoBehaviour
{
    public List<GameObject> CartoonImg;

    private void Start()
    {
        foreach (var img in CartoonImg)
        {
            img.SetActive(false);
        }
        StartCoroutine(Cartoon_Coroutine());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SceneManager.LoadScene("LoadingScene");
        }

        if (CartoonImg[CartoonImg.Count - 1].gameObject.activeSelf)
        {
            SceneManager.LoadScene("LoadingScene");
        }
    }

    IEnumerator Cartoon_Coroutine()
    {
        for (int i = 0; i < CartoonImg.Count; i++)
        {
            CartoonImg[i].SetActive(true);
            yield return new WaitForSeconds(3.5f);
            if (i < CartoonImg.Count - 1)
            {
                CartoonImg[i].SetActive(false);
            }
        }
    }
}
