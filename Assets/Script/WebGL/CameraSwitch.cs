using UnityEngine;
using TMPro;

public class CameraSwitch : MonoBehaviour
{
    // 카메라 배열 & 인덱스
    private Camera[] cameraArray = new Camera[3];
    private int activeCameraIndex = 0;

    // 객체 참조
    public CameraOverView cameraoverview; 
    public CameraObserver cameraobserver; 
    public CameraUser camerauser;
    public SetPositionNeb setpositionneb;
    // public UserPositionUpdate userpositionupdate;
    public CanvasToolBar canvastoolbar;
    public MouseCursor mousecursor;

    // 텍스트박스
    // public TextMeshProUGUI viewpointText; // 사용자 카메라의 시점 출력

    void Start()
    {
        cameraoverview = FindObjectOfType<CameraOverView>();
        cameraobserver = FindObjectOfType<CameraObserver>();
        camerauser = FindObjectOfType<CameraUser>();
        setpositionneb = FindObjectOfType<SetPositionNeb>();
        // userpositionupdate = FindObjectOfType<UserPositionUpdate>();
        canvastoolbar = FindObjectOfType<CanvasToolBar>();
        mousecursor = FindObjectOfType<MouseCursor>();

        // 카메라 할당 
        cameraArray[0] = cameraoverview.Overview;
        cameraArray[1] = cameraobserver.Observer;
        cameraArray[2] = camerauser.User;
    }

    void Update()
    {
        // 입력에 따라 카메라 전환
        if (Input.GetKeyDown(KeyCode.V)) // V 키를 눌렀을 때
        {
            SwitchCamera();
        }
        else if (cameraArray[activeCameraIndex].name == "Camera(Observer)")
        {
            if (!mousecursor.isAltPressed)
            {
                cameraobserver.MoveObserver();
            }
            canvastoolbar.viewpointText.text = "자유 시점";
        }
        else if (cameraArray[activeCameraIndex].name == "Camera(Overview)")
        {
            if (!mousecursor.isAltPressed)
            {
                cameraoverview.MoveOverView();
            }
            canvastoolbar.viewpointText.text = "탑다운 시점";
        }
        else if (cameraArray[activeCameraIndex].name == "Camera(User)")
        {
            // if (userpositionupdate.ClickedUserInfo == null) // 사용자 클릭하기 전.
            // {
            //     canvastoolbar.viewpointText.text = "사용자 시점";
            // }
            // else
            // {
            //     // 타겟 플레이어 층 정보 반영
            //     setpositionneb.FloorController = userpositionupdate.ClickedUserInfo.user_floor;
            //     // 인칭 텍스트 출력
            //     if (camerauser.isFirstPersonView)   {canvastoolbar.viewpointText.text = "1인칭 시점"; }
            //     else                                {canvastoolbar.viewpointText.text = "트레이싱 시점"; }
            // }
        }
        else
        {
            canvastoolbar.viewpointText.text = "";
        }
        // 옵저버 카메라 메인 카메라 위치로 이동.
        ObserverReposition();
    }

    void SwitchCamera()
    {
        if (camerauser.isFirstPersonView)
        {
            camerauser.oneTargetFlag = false;
        }
        // 현재 카메라 비활성화
        cameraArray[activeCameraIndex].enabled = false;

        // 인덱스를 다음 카메라로 이동
        activeCameraIndex = (activeCameraIndex + 1) % cameraArray.Length;

        // 다음 카메라 활성화
        cameraArray[activeCameraIndex].enabled = true;
    }

    public void ObserverReposition()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (cameraArray[activeCameraIndex].name == "Camera(Observer)")
            {
                // 현재 활성화된 카메라의 위치와 회전으로 이동
                cameraArray[activeCameraIndex].transform.position = cameraArray[(activeCameraIndex + 1 + cameraArray.Length) % cameraArray.Length].transform.position;
            }
        }
    }
}
