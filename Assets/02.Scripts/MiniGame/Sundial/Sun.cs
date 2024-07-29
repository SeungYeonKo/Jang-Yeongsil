using UnityEngine;
using UnityEngine.UI;

public class Sun : MonoBehaviour
{
    public Slider rotationSlider; // 슬라이더 UI를 참조
    private float minRotation = 160f; // 최소 회전 값
    private float maxRotation = 30f; // 최대 회전 값

    void Start()
    {
        if (rotationSlider != null)
        {
            rotationSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    void OnSliderValueChanged(float value)
    {
        // 슬라이더 값이 0에서 500으로 변할 때 회전 값을 160에서 30으로 매핑
        float rotationValue = Mathf.Lerp(minRotation, maxRotation, value / 500f);

        // 새로운 회전 값 설정, Y와 Z 값은 90도로 고정
        transform.rotation = Quaternion.Euler(rotationValue, 90f, 90f);
    }

    void Update()
    {
        // 필요시 Update에서 추가적인 로직 처리
    }

    private void OnDestroy()
    {
        if (rotationSlider != null)
        {
            rotationSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }
}
