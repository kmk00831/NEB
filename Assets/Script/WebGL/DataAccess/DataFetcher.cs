using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class RecentUserLocation
{
    public int mapId;
    public string userId;
    public float userX;
    public float userY;
    public int userZ;
    public float userDirection;
    public int userFloor;
    public string userStatus;
    public string timestamp;
}

public class DataFetcher : MonoBehaviour
{
    public List<RecentUserLocation> latestData = new(); // 다른 스크립트에서 접근 가능

    void Start()
    {
        StartCoroutine(GetRecentLocationsLoop());
    }

    IEnumerator GetRecentLocationsLoop()
    {
        while (true)
        {
            yield return StartCoroutine(GetRecentLocations());
            yield return new WaitForSeconds(1f); // 1초 간격
        }
    }

    public IEnumerator GetRecentLocations()
    {
        string url = "https://rnn.korea.ac.kr/express/locations/recent-all";
        string token = PlayerPrefs.GetString("AccessToken");

        UnityWebRequest req = UnityWebRequest.Get(url);
        req.SetRequestHeader("Authorization", "Bearer " + token);

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string rawJson = req.downloadHandler.text;

            // "data": [...] 부분만 파싱
            int dataStart = rawJson.IndexOf("\"data\":");
            if (dataStart != -1)
            {
                int arrayStart = rawJson.IndexOf("[", dataStart);
                int arrayEnd = rawJson.IndexOf("]", arrayStart);
                if (arrayStart != -1 && arrayEnd != -1)
                {
                    string arrayJson = rawJson.Substring(arrayStart, arrayEnd - arrayStart + 1);
                    RecentUserLocation[] parsed = DBJsonHelper.FromJson<RecentUserLocation>(arrayJson);
                    latestData = new List<RecentUserLocation>(parsed);
                }
                else
                {
                    Debug.LogWarning("⚠️ 'data' 배열 파싱 실패");
                }
            }
            else
            {
                Debug.LogWarning("⚠️ 'data' 필드 없음");
            }
        }
        else if (req.responseCode == 401)
        {
            Debug.LogWarning("❌ 토큰 만료 또는 누락 → 재발급 필요");
        }
        else
        {
            Debug.LogError("❌ 요청 실패: " + req.error);
        }
    }
}
