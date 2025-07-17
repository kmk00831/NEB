using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Vector3DataLog
{
    public float x;
    public float y;
    public float z;
}

[System.Serializable]
public class SensorDataLog
{
    public int mapId;
    public Vector3DataLog acceleration;
    public Vector3DataLog magnetic;
    public Vector3DataLog gyro;
    public Vector3DataLog linearAcceleration;
    public Vector3DataLog rotation;
    public float pressure;
    public float light;
    public float proximity;
    public float rf;
    public int userStateReal;
}

[System.Serializable]
public class SensorLogListWrapper
{
    public List<SensorDataLog> data;
}

public class SensorLogFetcher : MonoBehaviour
{
    private Coroutine logFetchCoroutine;

    /// <summary>
    /// 외부에서 호출: 지정된 유저 ID로 로그 수신 시작
    /// </summary>
    public void StartFetching(string userId)
    {
        if (logFetchCoroutine != null)
            StopCoroutine(logFetchCoroutine);

        logFetchCoroutine = StartCoroutine(GetLatestSensorFromLogLoop(userId)); // ✅ 반복 루프 코루틴 실행
    }

    /// <summary>
    /// 외부에서 호출: 로그 수신 중단
    /// </summary>
    public void StopFetching()
    {
        if (logFetchCoroutine != null)
        {
            StopCoroutine(logFetchCoroutine);
            logFetchCoroutine = null;
            Debug.Log("🛑 센서 로그 수신 중단됨");
        }
    }

    private IEnumerator GetLatestSensorFromLogLoop(string userId)
    {
        while (true)
        {
            string url = $"https://rnn.korea.ac.kr/express/sensors/{userId}?page=1&limit=500";
            string token = PlayerPrefs.GetString("AccessToken");

            UnityWebRequest req = UnityWebRequest.Get(url);
            req.SetRequestHeader("Authorization", "Bearer " + token);

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                string rawJson = req.downloadHandler.text;
                try
                {
                    var wrapper = JsonUtility.FromJson<SensorLogListWrapper>(rawJson);
                    var list = wrapper.data;

                    if (list != null && list.Count > 0)
                    {
                        var d = list[0];
                        Debug.Log($"📊 [{userId}] 로그 총 개수: {list.Count}");
                        // 여기서 d를 원하는 방식으로 처리
                    }
                    else
                    {
                        Debug.LogWarning($"📭 [{userId}] 로그 데이터 없음");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"❌ 로그 파싱 실패: {e.Message}");
                }
            }
            else
            {
                Debug.LogError($"❌ 로그 요청 실패: {req.error}");
            }
            yield return new WaitForSeconds(0.3f); 
        }
    }
}
