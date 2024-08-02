using UnityEngine;
using UnityEngine.UI;

public class SliderImageAnimator : MonoBehaviour
{
    public Slider imageSlider; // 슬라이더 컴포넌트
    public Image displayImage; // 이미지 컴포넌트
    public Sprite[] sprites; // 연속된 이미지 배열

    private void Start()
    {
        // 슬라이더의 최대 값을 이미지 배열의 길이 - 1로 설정
        imageSlider.maxValue = sprites.Length - 1;

        // 슬라이더의 초기값을 1로 설정
        imageSlider.value = 1;

        // 초기 이미지 설정
        displayImage.sprite = sprites[1];

        // 슬라이더의 값이 변경될 때마다 OnSliderValueChanged 함수 호출
        imageSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public void OnSliderValueChanged(float value)
    {
        // 슬라이더의 값에 따라 이미지 변경
        int index = Mathf.RoundToInt(value);
        displayImage.sprite = sprites[index];
    }

    // 미니게임이 시작될 때 이미지를 갱신하기 위해 외부에서 호출하는 메서드
    public void UpdateImage()
    {
        OnSliderValueChanged(imageSlider.value);
    }
}
