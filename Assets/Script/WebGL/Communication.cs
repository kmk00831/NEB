// using UnityEngine;
// using UnityEngine.Networking;
// using System.Collections;

// [System.Serializable]
// public class UserData
// {
//     public string user_dateTime;
//     public int user_floor;
//     public bool user_status;
//     public int user_uid;
//     public float user_x;
//     public float user_y;
//     public int user_statereal;
//     public System.Collections.Generic.List<UserLog> user_log;
// }

// [System.Serializable]
// public class UserLog
// {
//     public int user_floor;
//     public float user_x;
//     public float user_y;
//     public int user_statereal;
//     public string user_dateTime;
// }

// [System.Serializable]
// public class UserDataArray
// {
//     public UserData[] users;
// }

// public class Communication : MonoBehaviour
// {
//     public static UserData[] CurrentUsers { get; private set; }  // ✅ 외부 접근용

//     private string url;
//     private Setup setup;

//     void Awake()
//     {
//         setup = FindObjectOfType<Setup>();
//         url = $"http://163.152.52.123:3896/{setup.SceneName}/{setup.SceneName}-UserCur.json";
//         StartCoroutine(UpdateDataRoutine());
//     }

//     private IEnumerator UpdateDataRoutine()
//     {
//         while (true)
//         {
//             yield return GetData();
//             yield return new WaitForSeconds(0.5f);
//         }
//     }

//     private IEnumerator GetData()
//     {
//         using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
//         {
//             yield return webRequest.SendWebRequest();

//             if (webRequest.result == UnityWebRequest.Result.Success)
//             {
//                 string jsonResponse = webRequest.downloadHandler.text;
//                 ProcessJsonData(jsonResponse);
//             }
//             else
//             {
//                 Debug.LogError("❌ 사용자 데이터 요청 실패: " + webRequest.error);
//             }
//         }
//     }

//     private void ProcessJsonData(string jsonData)
//     {
//         try
//         {
//             string wrappedJson = "{\"users\":" + jsonData + "}";
//             UserDataArray userDataArray = JsonUtility.FromJson<UserDataArray>(wrappedJson);

//             if (userDataArray.users != null)
//             {
//                 CurrentUsers = userDataArray.users;
//             }
//             else
//             {
//                 Debug.LogWarning("⚠️ 수신된 JSON은 올바른 UserData 배열이 아님.");
//                 CurrentUsers = null;
//             }
//         }
//         catch (System.Exception ex)
//         {
//             Debug.LogError("❌ JSON 파싱 오류: " + ex.Message);
//         }
//     }
// }
