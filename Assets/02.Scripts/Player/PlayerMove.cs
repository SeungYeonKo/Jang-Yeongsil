using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    public float Speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    public Transform cameraTransform; // 카메라의 Transform

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;
    Vector3 dir = Vector3.zero;

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
            InputAndDir();

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
    void InputAndDir()
    {
        // 키 입력에 따라 방향 벡터 설정
        dir.x = Input.GetAxis("Horizontal");   // x축 방향 키 입력
        dir.z = Input.GetAxis("Vertical");     // z축 방향 키 입력
        Vector3 direction = new Vector3(dir.x, 0f, dir.z);
        float movementMagnitude = direction.magnitude;

        
        
        //Instantiate(WalkVFX, dir, Quaternion.identity);

        if (dir != Vector3.zero)   // 키 입력이 있는 경우
        {
            // 카메라의 앞 방향을 기준으로 이동 방향 설정
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0;
            direction = (forward.normalized * dir.z + Camera.main.transform.right * dir.x).normalized;

            var a = direction;
            a.y = 0f;
            // 이동 방향으로 캐릭터 회전
            Quaternion targetRotation = Quaternion.LookRotation(a);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);

            // 캐릭터 컨트롤러를 사용한 이동
            CharacterController controller = GetComponent<CharacterController>();
            controller.Move(direction * Time.deltaTime * Speed);
        }
    }


}
