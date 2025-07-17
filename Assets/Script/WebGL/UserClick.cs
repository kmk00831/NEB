using UnityEngine;
using TMPro;

public class UserClick : MonoBehaviour
{
    private Camera ObserverCamera; // 사용할 카메라를 Inspector에서 할당
    public LayerMask targetLayer; // 클릭 감지할 레이어를 Inspector에서 할당
    public delegate void ClickAction(); // 델리게이트 선언
    public static event ClickAction OnClicked; // 이벤트 선언
    public string number = null; // 웹으로 보낼 유저 id 저장
                                 // 객체 참조
    public CameraObserver cameraobserver;
    // public UserPositionUpdate userpositionupdate;
    // public UserReplay userreplay;
     // 자식 오브젝트 이름을 전달하기 위한 이벤트 추가
    public delegate void UserClickedAction(string userName);
    public static event UserClickedAction OnUserClicked;
    // 텍스트 박스
    // private TextMeshProUGUI objectInfoText;

    void Start()
    {
        cameraobserver = FindObjectOfType<CameraObserver>();
        // userpositionupdate = FindObjectOfType<UserPositionUpdate>();
        // userreplay = FindObjectOfType<UserReplay>();

        // if (userpositionupdate == null)
        // {
        //     Debug.LogError("UserPositionUpdate script not found in the scene!");
        // }

        // if (userreplay == null)
        // {
        //     Debug.LogError("UserReplay script not found in the scene!");
        // }

        // 사용자 정보 출력 텍스트 박스 
        // GameObject objectNameText = GameObject.Find("UserInfoTextBox");
        // objectInfoText = objectNameText.GetComponent<TextMeshProUGUI>();
        // objectInfoText.gameObject.SetActive(true);

        // 카메라 할당 옵저버
        ObserverCamera = cameraobserver.Observer;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭 시
        {
            Ray ray = ObserverCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
            {
                string[] parts = hit.collider.gameObject.name.Split('_'); // UID 수집
                number = parts[1]; // "_" 다음의 텍스트 추출
            }

            // if(userreplay.isReplay){
            //     objectInfoText.text = "UID: " + number + "\n" + "Floor: " + userreplay.ReplayFloor + "\n" + "Pos: " + userreplay.ReplayX + ", " + userreplay.ReplayY;
            // }
        }

        // if (userpositionupdate.ClickedUserInfo != null)
        // {
        //     if (userpositionupdate.ClickedUserInfo.user_status && userreplay.isReplay == false) // 캔버스에 클릭된 유저 데이터 출력
        //     {
        //         float CurPosX = userpositionupdate.ClickedUserInfo.user_x; // 앱에 저장되는 실 좌표값
        //         float CurPosY = userpositionupdate.ClickedUserInfo.user_y;  // 앱에 저장되는 실 좌표값
        //         int CurFloor = userpositionupdate.ClickedUserInfo.user_floor; // 현재 층

        //         // objectInfoText.text = "UID: " + number + "\n" + "Floor: " + CurFloor + "\n" + "Pos: " + CurPosX + ", " + CurPosY;
        //     }
        // }
    }
}
