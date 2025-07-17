using UnityEngine;

public class CameraObserver : MonoBehaviour
{
    // 카메라 변수
    public Camera Observer;
    // 카메라 이동 파라미터
    private float ObserverSpeed = 150f; // 기본 카메라 이동 속도
    private float ObserverSprintMultiplier = 3f; // 이동 속도 배수
    private float ObserverRotationSpeed = 2f; // 카메라 회전 속도
    private float ObserverZoomSpeed = 30f; // 확대/축소 속도
    private float ObserverMinY = -200f; // 최소 Y축 위치
    private float ObserverMaxY = 1000f; // 최대 Y축 위치
    private float ObserverFieldOfView = 60f; // 기본 시야 각도
    private float ObserverRotationX = 38f; // 수평 회전
    private float ObserverRotationY = 90f; // 수직 회전
    private float ObserverCurrentY; // 현재 Y축 위치
    public bool isMouseActive = true;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 잠금
        ObserverCurrentY = transform.position.y; // 시작 시 Y축 위치 저장
        // 카메라 할당
        GameObject cameraObject = GameObject.Find("Camera(Observer)");
        Observer = cameraObject.GetComponent<Camera>();
    }

    
    public void MoveObserver()
    {           
        // X축과 Z축 이동
        float moveHorizontal = Input.GetAxis("Horizontal"); // A/D 키
        float moveVertical = Input.GetAxis("Vertical"); // W/S 키

        // 카메라의 앞 방향을 기준으로 이동 벡터 계산
        Vector3 forward = Observer.transform.TransformDirection(Vector3.forward);
        Vector3 right = Observer.transform.TransformDirection(Vector3.right);
        Vector3 movement = (forward * moveVertical + right * moveHorizontal).normalized;

        // 이동 속도 조정
        float currentSpeed = ObserverSpeed;
        if (Input.GetKey(KeyCode.LeftShift)) // 왼쪽 Shift 키 체크
        {
            currentSpeed *= ObserverSprintMultiplier; // 속도를 3배로 증가
        }

        // Y축 위치를 유지하며 이동
        Vector3 newPosition = Observer.transform.position + movement * currentSpeed * Time.deltaTime;
        newPosition.y = ObserverCurrentY; // Y축 위치를 현재 Y로 유지
        Observer.transform.position = newPosition; // Observer의 위치 업데이트

        // Y축 위치 조정 (마우스 휠)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            float newY = ObserverCurrentY - scroll * ObserverZoomSpeed;
            newY = Mathf.Clamp(newY, ObserverMinY, ObserverMaxY); // Y축 위치 제한
            ObserverCurrentY = newY; // 현재 Y 업데이트
            Observer.transform.position = new Vector3(Observer.transform.position.x, ObserverCurrentY, Observer.transform.position.z);
        }

        if (isMouseActive){
            // 회전
            float mouseX = Input.GetAxis("Mouse X") * ObserverRotationSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * ObserverRotationSpeed;

            ObserverRotationX -= mouseY; // 수직 회전
            ObserverRotationX = Mathf.Clamp(ObserverRotationX, -90f, 90f); // 수직 회전 제한
            ObserverRotationY += mouseX; // 수평 회전

            Observer.transform.localRotation = Quaternion.Euler(ObserverRotationX, ObserverRotationY, 0f); // 회전 적용
        }
        
    }
}