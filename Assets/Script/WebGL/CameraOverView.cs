using UnityEngine;

public class CameraOverView : MonoBehaviour
{
    // 카메라 변수
    public Camera Overview; // 특정 카메라 오브젝트
    // 카메라 파라미터
    private float OverViewMoveSpeed = 150f; // 카메라 이동 속도
    private float OverViewZoomSpeed = 0.1f;   // 카메라 확대/축소 속도
    private float OverViewMinZoom = 5f;     // 최소 줌
    private float OverViewMaxZoom = 80f;    // 최대 줌

    void Start()
    {
        // 카메라 할당
        GameObject cameraObject = GameObject.Find("Camera(Overview)");
        Overview = cameraObject.GetComponent<Camera>();
    }
    public void MoveOverView()
    {
        float horizontal = Input.GetAxis("Horizontal") * OverViewMoveSpeed * Time.deltaTime; // A, D 키로 X축 이동
        float vertical = Input.GetAxis("Vertical") * OverViewMoveSpeed * Time.deltaTime; // W, S 키로 Z축 이동

        transform.Translate(horizontal, vertical, 0);

        // 카메라 확대/축소
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            Overview.fieldOfView -= scroll * OverViewZoomSpeed * 100f; // Time.deltaTime 제거
            Overview.fieldOfView = Mathf.Clamp(Overview.fieldOfView, OverViewMinZoom, OverViewMaxZoom);
        }
    }
}