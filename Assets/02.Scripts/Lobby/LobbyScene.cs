using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : MonoBehaviour
{    
    public GameObject LoginPopup;
    void Start()
    {
        StartCoroutine(Show_Coroutine());
    }
    IEnumerator Show_Coroutine()
    {
        LoginPopup.SetActive(false);
        yield return new WaitForSeconds(5f);
        LoginPopup.SetActive(true);
    }
}
