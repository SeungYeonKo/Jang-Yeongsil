using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UI_CharacterSelect : MonoBehaviour
{
    public Button MaleButton;
    public Button FemaleButton;
    // Start is called before the first frame update
    void Start()
    {
        MaleButton.onClick.AddListener(() => SelectGender("Male"));
        FemaleButton.onClick.AddListener(() => SelectGender("Female"));
    }
    private void SelectGender(string gender)
    {
        // 선택된 성별을 저장
        PlayerPrefs.SetString("SelectedGender", gender);
        PlayerPrefs.Save();

        Debug.Log($"Selected Gender: {gender}");

        string userId = PlayerPrefs.GetString("LoggedInId", string.Empty);
        if (!string.IsNullOrEmpty(userId))
        {
            PersonalManager.Instance.UpdateGender(userId, gender);
        }
    }

    public void OnClickStartButton()
    {
        PhotonNetwork.LoadLevel("VillageScene");
    }
}
