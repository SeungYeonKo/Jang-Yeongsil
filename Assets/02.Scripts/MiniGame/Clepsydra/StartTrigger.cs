using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartTrigger : MonoBehaviour
{
    public Image StartImage;
    public Button StartButton;
    public Transform Maze1StartPosition;

    private GameObject player;

    private void Start()
    {
        StartButton.onClick.AddListener(OnStartButtonClick);
    }

    public void OnStartButtonClick()
    {
        if (player != null && Maze1StartPosition != null)
        {
            player.transform.position = Maze1StartPosition.position;
        }

        if (StartImage != null)
        {
            StartImage.gameObject.SetActive(false);
        }

        if (StartButton != null)
        {
            StartButton.gameObject.SetActive(false);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (StartImage != null)
            {
                StartImage.gameObject.SetActive(true);
            }

            if (StartButton != null)
            {
                StartButton.gameObject.SetActive(true);
            }

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (StartImage != null)
            {
                StartImage.gameObject.SetActive(false);
            }

            if (StartButton != null)
            {
                StartButton.gameObject.SetActive(false);
            }

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            player = null;
        }
    }
}
