using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Vector3Data
{
    public float x;
    public float y;
    public float z;
}

[System.Serializable]
public class SensorData
{
    public int mapId;
    public Vector3Data acceleration;
    public Vector3Data magnetic;
    public Vector3Data gyro;
    public Vector3Data linearAcceleration;
    public Vector3Data rotation;
    public float pressure;
    public float light;
    public float proximity;
    public float rf;
    public int userStateReal;
}// 필요 시 필드 추가


public class SensorFetcher : MonoBehaviour
{
    // 참조
    public CanvasSideBar canvassidebar;
    public Dictionary<string, SensorData> sensorDataByUserId = new();

    void Start()
    {
        canvassidebar = FindObjectOfType<CanvasSideBar>();
        StartCoroutine(GetAllUserSensorsLoop());
    }

    IEnumerator GetAllUserSensorsLoop()
    {
        while (true)
        {
            if (!string.IsNullOrEmpty(canvassidebar.ClickedUserID))
            {
                yield return StartCoroutine(GetSensorForUser(canvassidebar.ClickedUserID));
                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                yield return null; // 한 프레임 대기 후 다시 검사
            }
        }
    }

    IEnumerator GetSensorForUser(string userId)
    {
        string url = $"https://rnn.korea.ac.kr/express/sensors/recent/{userId}";
        string token = PlayerPrefs.GetString("AccessToken");

        UnityWebRequest req = UnityWebRequest.Get(url);
        req.SetRequestHeader("Authorization", "Bearer " + token);

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string rawJson = req.downloadHandler.text;
            // Debug.Log("🌐 rawJson:\n" + rawJson);
            if (rawJson.Contains("\"data\":null"))
            {
                Debug.Log($"⚠️ [{userId}] 센서 데이터 없음.");
                yield break;
            }

            try
            {
                var wrapper = JsonUtility.FromJson<SensorWrapper>(rawJson); // SensorWrapper에 data 필드 필요
                sensorDataByUserId[userId] = wrapper.data;
                var d = wrapper.data;
                
                // Debug.Log(
                //     $"✅ [{userId}] 센서 수신 전체 데이터:\n" +
                //     $"• mapId: {d.mapId}\n" +
                //     $"• acceleration: ({d.acceleration.x}, {d.acceleration.y}, {d.acceleration.z})\n" +
                //     $"• magnetic: ({d.magnetic.x}, {d.magnetic.y}, {d.magnetic.z})\n" +
                //     $"• gyro: ({d.gyro.x}, {d.gyro.y}, {d.gyro.z})\n" +
                //     $"• linearAcceleration: ({d.linearAcceleration.x}, {d.linearAcceleration.y}, {d.linearAcceleration.z})\n" +
                //     $"• rotation: ({d.rotation.x}, {d.rotation.y}, {d.rotation.z})\n" +
                //     $"• pressure: {d.pressure}\n" +
                //     $"• light: {d.light}\n" +
                //     $"• proximity: {d.proximity}\n" +
                //     $"• rf: {d.rf}\n" +
                //     $"• userStateReal: {d.userStateReal}"
                // );
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ [{userId}] 파싱 실패: {e.Message}");
            }
        }
        else
        {
            Debug.LogError($"❌ [{userId}] 요청 실패: {req.error}");
        }
    }

    bool DataFetcherInstanceAvailable(out DataFetcher fetcher)
    {
        fetcher = FindObjectOfType<DataFetcher>();
        return fetcher != null;
    }

    [System.Serializable]
    public class SensorWrapper
    {
        public SensorData data;
    }

    public void RequestSensorForUser(string userId, System.Action<int?> onComplete)
    {
        StartCoroutine(GetSensorForUserWithCallback(userId, onComplete));
    }

    private IEnumerator GetSensorForUserWithCallback(string userId, System.Action<int?> onComplete)
    {
        string url = $"https://rnn.korea.ac.kr/express/sensors/recent/{userId}";
        string token = PlayerPrefs.GetString("AccessToken");

        UnityWebRequest req = UnityWebRequest.Get(url);
        req.SetRequestHeader("Authorization", "Bearer " + token);

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string rawJson = req.downloadHandler.text;

            if (!rawJson.Contains("\"data\":null"))
            {
                var wrapper = JsonUtility.FromJson<SensorWrapper>(rawJson);
                sensorDataByUserId[userId] = wrapper.data;
                onComplete?.Invoke(wrapper.data.userStateReal); // ✅ 콜백 실행
                yield break;
            }
        }

        // 실패한 경우
        onComplete?.Invoke(null);
    }
}
