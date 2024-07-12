using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    public Transform cameraTransform; // 카메라의 Transform

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform; // 카메라 Transform을 지정하지 않았을 경우 메인 카메라 사용
        }
    }

    void Update()
    {
        if (controller.isGrounded)
        {
            // 지면에 있을 때만 WASD로 이동
            Vector3 inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            if (inputDirection.magnitude > 0.1f)
            {
                // 카메라의 방향을 기준으로 이동 방향 설정
                Vector3 forward = cameraTransform.forward;
                Vector3 right = cameraTransform.right;

                forward.y = 0;
                right.y = 0;

                forward.Normalize();
                right.Normalize();

                moveDirection = forward * inputDirection.z + right * inputDirection.x;
                moveDirection *= speed;

                // 카메라의 방향으로 캐릭터 회전
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            }
            else
            {
                moveDirection = Vector3.zero;
            }

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        // 중력 적용
        moveDirection.y -= gravity * Time.deltaTime;

        // 캐릭터 이동
        controller.Move(moveDirection * Time.deltaTime);
    }
}
