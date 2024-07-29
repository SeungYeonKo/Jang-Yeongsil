using UnityEngine;
using UnityEngine.UI;

public class SliderImageAnimator : MonoBehaviour
{
    public Slider imageSlider; // 슬라이더 컴포넌트
    public Image displayImage; // 이미지 컴포넌트
    public Sprite[] sprites; // 연속된 이미지 배열

    private void Start()
    {
        // 슬라이더의 값이 변경될 때마다 OnSliderValueChanged 함수 호출
        imageSlider.onValueChanged.AddListener(OnSliderValueChanged);

        // 슬라이더의 최대 값을 이미지 배열의 길이 - 1로 설정
        imageSlider.maxValue = sprites.Length - 1;

        // 초기 이미지 설정
        displayImage.sprite = sprites[0];
    }

    private void OnSliderValueChanged(float value)
    {
        // 슬라이더의 값에 따라 이미지 변경
        int index = Mathf.RoundToInt(value);
        displayImage.sprite = sprites[index];
    }
}
