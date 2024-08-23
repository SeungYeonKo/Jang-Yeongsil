using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class StartTrigger : MonoBehaviour
{
    public Image StartImage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(StartImage != null)
            {
                StartImage.gameObject.SetActive(true);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(StartImage != null)
            {
                StartImage.gameObject.SetActive(false); 
            }
        }
    }
}
