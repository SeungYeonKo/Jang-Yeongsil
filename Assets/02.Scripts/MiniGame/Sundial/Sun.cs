using UnityEngine;
using UnityEngine.UI;

public class Sun : MonoBehaviour
{
    public Slider rotationSlider; // 슬라이더 UI를 참조
    private float minRotation = 160f; // 최소 회전 값
    private float maxRotation = 13f; // 최대 회전 값

    void Start()
    {
        if (rotationSlider != null)
        {
            rotationSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    void OnSliderValueChanged(float value)
    {
        // 슬라이더 값에 따라 회전 값을 계산
        float rotationValue = Mathf.Lerp(minRotation, maxRotation, value);

        // 새로운 회전 값 설정
        transform.rotation = Quaternion.Euler(rotationValue, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
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
