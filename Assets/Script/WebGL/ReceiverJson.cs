// using UnityEngine;
// using UnityEngine.Networking;
// using System.Collections;
// using System.Collections.Generic;

// [System.Serializable]
// public class UserLogData
// {
//     public int user_uid;
//     public List<SpecificUserLog> user_log;
// }

// [System.Serializable]
// public class SpecificUserLog
// {
//     public int user_floor;
//     public float user_x;
//     public float user_y;
//     public int user_statereal;
//     public string user_dateTime;
// }

// [System.Serializable]
// public class UserLogDataArray
// {
//     public UserLogData[] users;
// }

// public class ReceiverJson : MonoBehaviour
// {
//     private string UrlBase = "http://163.152.52.123:3896/NEB/NEB-UserLog"; // 기본 주소임 여기에 선택된 유저 아이디 추가
//     public UserLogData ClickedUserInfo;
//     public UserClick userclick;
//     public UserReplay userreplay;
//     public Receiver receiver;
//     private int previousReplayUserID = 0; // 이전 ReplayUserID 저장

//     void Start()
//     {
//         userreplay = FindObjectOfType<UserReplay>();
//         userclick = FindObjectOfType<UserClick>();
//         receiver = FindObjectOfType<Receiver>();
//         StartCoroutine(CheckUserIDRoutine());
//     }

//     // ReplayUserID가 변경되었는지 확인하는 코루틴
//     private IEnumerator CheckUserIDRoutine()
//     {
//         while (true)
//         {
//             if (receiver.ReplayUserID != previousReplayUserID)
//             {
//                 previousReplayUserID = receiver.ReplayUserID; // 값 업데이트
//                 StopCoroutine(UpdateDataRoutine()); // 기존 코루틴 중지
//                 StartCoroutine(UpdateDataRoutine()); // 새 코루틴 시작
//             }
//             yield return null; // 매 프레임 확인
//         }
//     }

//     private IEnumerator UpdateDataRoutine()
//     {
//         yield return GetData();
//         yield return new WaitForSeconds(0.5f); // 0.5초마다 업데이트  
//     }

//     private IEnumerator GetData()
//     {
//         string url = string.Format("{0}/USER_{1}.json", UrlBase, receiver.ReplayUserID); // 베이스 url, 파일이름 통합
//         using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
//         {
//             yield return webRequest.SendWebRequest();

//             if (webRequest.result == UnityWebRequest.Result.Success){ // 성공 시
//                 string jsonResponse = webRequest.downloadHandler.text;
//                 ProcessJsonLogData(jsonResponse);
//             }
//             else{
//                 // 여기로 들어오면 아직 유니티에서 특정 오브젝트(사용자) 클릭 안 한거임
//                 yield break;
//             }
//         }
//     }

//     private void ProcessJsonLogData(string jsonData)
//     {
//         try
//         {
//             UserLogDataArray userLogDataArray = JsonUtility.FromJson<UserLogDataArray>("{\"users\":" + jsonData + "}");

//             if (userLogDataArray.users != null)
//             {
//                 foreach (UserLogData user in userLogDataArray.users)
//                 {
//                     if(receiver.ReplayUserID == user.user_uid) // 가져온 uid가 일치하고 가져온 uid와 이전 uid가 일치하지 않으면 로그 값 전송
//                     {
//                         Debug.Log(user.user_log);
//                         userreplay.SetUserLogs(user.user_log);
//                         break;
//                     }
//                 }
//             }
//             else
//             {
//                 Debug.LogError("JSON data is not a valid UserData array.");
//             }
//         }
//         catch (System.Exception ex)
//         {
//             Debug.LogError("JSON 1parse error: " + ex.Message);
//         }
//     }

// }

