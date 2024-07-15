using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class UI_CharacterSelect : MonoBehaviour
{
    public Button MaleButton;
    public Button FemaleButton;
    public GameObject MaleCharacter;
    public GameObject FemaleCharacter;
    public Animator MaleAnimator;
    public Animator FemaleAnimator;
    public Camera MaleCamera;
    public Camera FemaleCamera;
    public GameObject StartButton;
    void Start()
    {
        MaleButton.onClick.AddListener(() => SelectGender(CharacterGender.Male));
        FemaleButton.onClick.AddListener(() => SelectGender(CharacterGender.Female));

        // 캐릭터 값이 있으면 ReloadCharacter 호출
        ReloadCharacter();
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
        MaleCamera.gameObject.SetActive(false);
        FemaleCamera.gameObject.SetActive(false);
        // 성별에 따른 캐릭터 및 VFX 처리
        if (gender == CharacterGender.Male)
        {
            StartCoroutine(PlayCharacterSequence(MaleCharacter, MaleAnimator, MaleCamera));
        }
        else
        {
            StartCoroutine(PlayCharacterSequence(FemaleCharacter, FemaleAnimator, FemaleCamera));
        }
    }

    private IEnumerator PlayCharacterSequence(GameObject character, Animator animator, Camera camera)
    {
        // 카메라 활성화
        camera.gameObject.SetActive(true);
        // 달리기 애니메이션 시작
        animator.SetTrigger("Run");
        Vector3 originalPosition = character.transform.position;
        Vector3 targetPosition = new Vector3(originalPosition.x, originalPosition.y, originalPosition.z + 5);
        float duration = 1f;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            character.transform.position = Vector3.Lerp(originalPosition, targetPosition, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        // 좋아하기 애니메이션 시작
        animator.SetTrigger("Win");
        
        yield return new WaitForSeconds(2f); // 좋아하기 애니메이션 시간 조정
        StartButton.SetActive(true);
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

                // 로드된 성별에 따라 UI를 업데이트하거나 다른 작업 수행
                if (gender == CharacterGender.Male)
                {
                    MaleButton.onClick.Invoke();
                }
                else if (gender == CharacterGender.Female)
                {
                    FemaleButton.onClick.Invoke();
                }
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
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 20,
            IsVisible = true,
            IsOpen = true,
            EmptyRoomTtl = 1000 * 20,
        };

        PhotonNetwork.JoinOrCreateRoom("Main", roomOptions, TypedLobby.Default);
        PhotonNetwork.LoadLevel("LoadingScene");
    }
}
