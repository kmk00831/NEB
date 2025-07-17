using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class POIExtractor : MonoBehaviour
{
    private string baseUrl = "http://localhost:3001"; // Express 서버 주소

    // 특정 태그("POI")를 가진 오브젝트들의 좌표 수집 및 전송
    void Start()
    {
        InvokeRepeating(nameof(StartSendRoutine), 2f, 10f);
    }

    void StartSendRoutine()
    {
        StartCoroutine(SendOnlyNewPOIs());
    }

    public IEnumerator SendOnlyNewPOIs()
    {
        // 1. 서버에서 기존 POI 목록을 가져온다
        string getUrl = $"{baseUrl}/poi/list";
        UnityWebRequest getRequest = UnityWebRequest.Get(getUrl);
        getRequest.SetRequestHeader("Content-Type", "application/json");

        yield return getRequest.SendWebRequest();

        if (getRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("서버에서 POI 목록을 불러오는 데 실패함: " + getRequest.error);
            yield break;
        }

        List<POIData> existingPOIs = JsonConvert.DeserializeObject<List<POIData>>(getRequest.downloadHandler.text);
        HashSet<string> existingNames = new HashSet<string>();
        foreach (var poi in existingPOIs)
        {
            existingNames.Add(poi.name);
        }

        // 2. Unity에서 "POI" 태그를 가진 오브젝트들 가져오기
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("POI");

        List<POIData> newPOIs = new List<POIData>();
        foreach (GameObject obj in taggedObjects)
        {
            if (!existingNames.Contains(obj.name))
            {
                POIData poi = new POIData
                {
                    name = obj.name,
                    x = obj.transform.position.x,
                    y = obj.transform.position.y,
                    z = obj.transform.position.z,
                    tag = "POI"
                };
                newPOIs.Add(poi);
            }
        }

        if (newPOIs.Count == 0)
        {
            Debug.Log("새로 등록할 POI 없음");
            yield break;
        }

        // 3. 서버로 신규 POI들 전송
        string jsonData = JsonUtility.ToJson(new POIDataList { pois = newPOIs });
        Debug.Log($"Unity → 서버로 전송할 신규 POI 데이터: {jsonData}");

        UnityWebRequest postRequest = new UnityWebRequest($"{baseUrl}/poi/save-multiple", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        postRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        postRequest.downloadHandler = new DownloadHandlerBuffer();
        postRequest.SetRequestHeader("Content-Type", "application/json");

        yield return postRequest.SendWebRequest();

        if (postRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("신규 POI 오브젝트 좌표 전송 성공!");
        }
        else
        {
            Debug.LogError($"좌표 전송 실패: {postRequest.error}");
        }
    }

}
