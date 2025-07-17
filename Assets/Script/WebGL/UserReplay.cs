// /*
//     리플레이 조작 방법
//     P: 리플레이 시작 & 재시작
//     O: 리플레이 중지 
//     U: 리플레이 종료
//     I: 리플레이 배속 
// */
// using UnityEngine;
// using System.Collections; // 코루틴 사용을 위해 추가
// using System.Collections.Generic; // List<T>를 사용하기 위해 추가
// using System.Diagnostics; // Stopwatch 사용을 위해 추가
// using TMPro;

// public class UserReplay : MonoBehaviour
// {
//     // 객체 참조
//     public SetPositionNeb setpositionneb;
//     public ReceiverJson receiverjson;
//     public Receiver receiver;
//     public UserClick userclick;
//     // 로그 리스트
//     public List<SpecificUserLog> receivedUserLogs = new List<SpecificUserLog>();

//     // 리플레이 파라미터
//     private string prehour;
//     private string premin;
//     private string presec;
//     private string premsec;
//     private bool isSecondSequence = false;
//     public bool isReplay = false;
//     private bool replayReset= false; // true시 모든 리플레이 오브젝트 초기화
//     private string objectNamePrefix;
//     private int ReplaySpeed = 1;
//     private bool isPaused = false;
//     public float ReplayX;
//     public float ReplayY;
//     public int ReplayFloor;
//     // 부모 오브젝트 & 코루틴 객체 할당
//     private Transform parentObject;
//     private Coroutine replayCoroutine;



//     void Start()
//     {
//         setpositionneb = FindObjectOfType<SetPositionNeb>();
//         receiverjson = FindObjectOfType<ReceiverJson>();
//         receiver = FindObjectOfType<Receiver>();
//         userclick = FindObjectOfType<UserClick>();

//         // 리플레이 정보 출력 창 비활성화
//         // GameObject textBoxObject = GameObject.Find("ReplayInfoTitleTextBox");
//         // ReplayInfoTitleTextBox = textBoxObject.GetComponent<TextMeshProUGUI>();
//         // ReplayInfoTitleTextBox.gameObject.SetActive(false);

//         // 부모 오브젝트 할당
//         GameObject foundGameObject = GameObject.Find("GameObject");
//         parentObject = foundGameObject.transform;
//     }
//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.P)) // P 키를 눌렀을 때
//         {
//             isReplay = true;
//             replayReset = false;
//             replayCoroutine = StartCoroutine(ReplayRunCoroutine(receivedUserLogs)); // 코루틴 시작
//         }
//         if (Input.GetKeyDown(KeyCode.O)) // O 키를 눌렀을 때
//         {
//             isPaused = !isPaused;
//         }
//         if (Input.GetKeyDown(KeyCode.U)) // U 키를 눌렀을 때
//         {
//             // 리플레이 정보 출력 창 비활성화
//             // ReplayInfoTitleTextBox.gameObject.SetActive(false);

//             isReplay = false;
//             replayReset = true;
//             objectNamePrefix = $"USER_{receiver.ReplayUserID}";
            
//             foreach (Transform child in parentObject)
//             {
//                 if (child.name.StartsWith(objectNamePrefix)) // 이름이 특정 접두사로 시작하는지 확인
//                 {
//                     Destroy(child.gameObject);
//                     UnityEngine.Debug.Log($"Deleted: {child.name}");
//                 }
//             }

//             if (replayCoroutine != null) // 코루틴이 실행 중인 경우
//             {
//                 StopCoroutine(replayCoroutine); // 코루틴 중단
//             }
//         }
//         if (Input.GetKeyDown(KeyCode.I)){
//                 ReplaySpeed++;
//                 if (ReplaySpeed > 3){
//                     ReplaySpeed = 1;
//                 }
//             }
//     }

//     public void SetUserLogs(List<SpecificUserLog> userLogs) // 선택된 오프라인 유저의 로그 가져오기
//     {
//         receivedUserLogs = userLogs; 
//     }

//     private IEnumerator ReplayRunCoroutine(List<SpecificUserLog> UserLogs)
//     {
//         Stopwatch sw = new Stopwatch(); // Stopwatch 객체 생성
//         ReplaySpeed = 1;
        
//         foreach (SpecificUserLog log in UserLogs)
//         {
//             if(replayReset){yield break;}

//             while (isPaused)
//             {
//                 // ReplayInfoTitleTextBox.text = "중지";
//                 yield return null; // 다음 프레임까지 대기
//             }

//             string result = $"{log.user_x + 1351.0}\t{log.user_y + 603.0}\t{log.user_floor}\t{0.0}"; 
//             setpositionneb.ShowMyPosition(result, receiver.ReplayUserID, log.user_statereal);

//             //시간 딜레이
//             string dateTimeString = log.user_dateTime;
            
//             string hour = dateTimeString.Substring(11, 2); // 시간
//             string min = dateTimeString.Substring(14, 2); // 분
//             string sec = dateTimeString.Substring(17, 2); // 초
//             string msec = dateTimeString.Substring(20, 3); // 밀리초
            
//             int delayMilliseconds = 0;
//             if (isSecondSequence == true)
//             {
//                 int h = int.Parse(hour) - int.Parse(prehour);
//                 int m = int.Parse(min) - int.Parse(premin);
//                 int s = int.Parse(sec) - int.Parse(presec);
//                 int ms = int.Parse(msec) - int.Parse(premsec);

//                 h = h * 60 * 60 * 1000;
//                 m = m * 60 * 1000;
//                 s = s * 1000;
//                 if(ms < 0){ms = ms * -1;}
//                 delayMilliseconds = ms + h + m + s; // 어찌저찌 시간 차 ms로 계산해서 그만 큼 보폭 시간 줌
//             }
            
//             prehour = hour;
//             premin = min;
//             presec = sec;
//             premsec = msec;

//             isSecondSequence = true;

//             // 딜레이 적용 (Stopwatch 사용)
//             sw.Restart();

//             switch (ReplaySpeed){
//                 case 2:
//                     delayMilliseconds = delayMilliseconds / 2;
//                     break;
//                 case 3:
//                     delayMilliseconds = delayMilliseconds / 3;
//                     break;
//                 default:
//                     break;
//             } 
//             while (sw.ElapsedMilliseconds < delayMilliseconds)
//             {
//                 yield return null; // 다음 프레임까지 대기
//             }
//             sw.Stop();

//             // // 리플레이 정보 출력 창 활성화 (Null Check)
//             // if (ReplayInfoTitleTextBox != null)
//             // {
//             //     ReplayInfoTitleTextBox.gameObject.SetActive(true);
//             // }   
//             // ReplayInfoTitleTextBox.text = "리플레이 중..." + " x" + ReplaySpeed;

//             ReplayX = log.user_x;
//             ReplayY = log.user_y;
//             ReplayFloor = log.user_floor;
//             // userclick.objectNameText.text = "UID: " + receiver.ReplayUserID + "\n" + "Floor: " + log.user_floor + "\n" + "Pos: " + log.user_x + ", " + log.user_y;
//         }
//     }
// }
