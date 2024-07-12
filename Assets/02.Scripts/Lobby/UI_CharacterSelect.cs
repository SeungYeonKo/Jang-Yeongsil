using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Unity.VisualScripting;

public class UI_CharacterSelect : MonoBehaviour
{
    public Button MaleButton;
    public Button FemaleButton;
    void Start()
    {
        MaleButton.onClick.AddListener(() => SelectGender(CharacterGender.Male));
        FemaleButton.onClick.AddListener(() => SelectGender(CharacterGender.Female));
    }


    private void SelectGender(CharacterGender gender)
    {
        // 선택된 성별을 저장
        PlayerPrefs.SetString("SelectedGender", gender.ToString());
        PlayerPrefs.Save();

        Debug.Log($"Selected Gender: {gender}");

        string userId = PlayerPrefs.GetString("LoggedInId", string.Empty);
        if (!string.IsNullOrEmpty(userId))
        {
            PersonalManager.Instance.UpdateGender(userId, gender);
        }
    }

    private void ReloadCharacter()
    {
        string userId = PlayerPrefs.GetString("LoggedInId", string.Empty);
        if (!string.IsNullOrEmpty(userId))
        {
            CharacterGender? gender = PersonalManager.Instance.ReloadGender(userId);
            if (gender != null)
            {
                Debug.Log($"Loaded Gender: {gender}");
                // 로드된 성별을 기반으로 UI를 업데이트하거나 다른 작업 수행
            }
            else
            {
                Debug.Log("User not found or gender not set.");
            }
        }
        else
        {
            Debug.LogError("User ID is not set in PlayerPrefs");
        }
    }

    public void OnClickStartButton()
    {
        PhotonNetwork.LoadLevel("MainScene");
    }
}
