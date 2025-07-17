// using UnityEngine;
// using System.Runtime.InteropServices;
// using Unity.VisualScripting;

// public class Receiver : MonoBehaviour
// {
//     public UserClick userclick; // UserClick 참조
//     public string preUserNumber;
//     public int ReplayUserID;
//     public CameraObserver cameraobserver;

//     void Start()
//     {
//         userclick = FindObjectOfType<UserClick>();
//         cameraobserver = FindObjectOfType<CameraObserver>();
//     }
//     void Update()
//     {
//         if (preUserNumber != userclick.number)
//         {
//             UnityToWeb();
//             preUserNumber = userclick.number;
//         }
//     }

//     void UnityToWeb() // 유니티에서 웹으로 데이터 전송
//     {
//         Application.ExternalCall("callUnityFunction", userclick.number);
//     }

//     // 유니티 Keyboard 활성화 여부 웹에서 유니티 애플리케이션 화면을 클릭하면 활성화, 그 외 비활성화. 토글할까햇는데 그냥 따로 만듬. 나중에 하셈.
//     public void UnityActivate()
//     {
//         WebGLInput.captureAllKeyboardInput = true;
//         Cursor.visible = false; // 마우스 커서 표시
//         cameraobserver.isMouseActive = true;
//         Debug.Log("유니티 키보드 & 마우스 입력이 활성화되었습니다."); // 로그 추가
//     }
//     public void UnityDeactivate()
//     {
//         WebGLInput.captureAllKeyboardInput = false;
//         Cursor.visible = true; // 마우스 커서 숨기기
//         cameraobserver.isMouseActive = false;
//         Debug.Log("유니티 키보드 & 마우스 입력이 비활성화되었습니다."); // 로그 추가

//     }
//     public void UserReplayer(int data) {ReplayUserID = data;} // 웹에서 클릭된 유저 id 수신
// }