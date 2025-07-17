using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class POIManager : MonoBehaviour
{
    private string baseUrl = "http://localhost:5000"; // Express 서버 주소
    public GameObject poiPrefab;
    public Transform parentObject;

    private Dictionary<int, POIData> poiDataCache = new Dictionary<int, POIData>(); // 이전 데이터를 저장하여 비교
    private Dictionary<int, GameObject> poiObjects = new Dictionary<int, GameObject>(); // Unity 오브젝트 관리
    bool IsPOIDifferent(POIData a, POIData b) 
    { 
        return a.x != b.x || a.y != b.y || a.z != b.z || a.tag != b.tag; 
    }
    
    void Start()
    {
        StartCoroutine(GetPOIList());
        InvokeRepeating(nameof(GetPOIList), 5f, 5f); // 5초마다 최신 데이터 요청
    }

    IEnumerator GetPOIList()
    {
        string url = $"{baseUrl}/poi/poi/list";

        //Debug.Log($"Unity → 서버로 POI 목록 요청: {url}");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                List<POIData> poiList = JsonConvert.DeserializeObject<List<POIData>>(jsonResponse);

                //Debug.Log($"서버 응답 (POI 데이터): {jsonResponse}");

                CompareAndUpdatePOIs(poiList);
            }
            else
            {
                Debug.LogError($"POI 목록 가져오기 실패: {request.error}");
            }
        }
    }

    void CompareAndUpdatePOIs(List<POIData> newPoiList)
    {
        foreach (var newPOI in newPoiList)
        {
            if (poiDataCache.ContainsKey(newPOI.id))
            {
                POIData oldPOI = poiDataCache[newPOI.id];
                // 값이 완전히 같다면 아무것도 하지 않음
                if (IsPOIDifferent(oldPOI, newPOI))
                {
                    Debug.Log($"POI 값 변경 감지 - ID: {newPOI.id}");

                    // 위치 변경
                    if (oldPOI.x != newPOI.x || oldPOI.y != newPOI.y || oldPOI.z != newPOI.z)
                    {
                        UpdatePOI(newPOI);
                    }

                    // 태그 변경
                    if (oldPOI.tag != newPOI.tag)
                    {
                        UpdatePOITag(newPOI);
                    }
                    
                }
            }
            else
            {
                // 새로운 ID → POI 생성
                CreatePOI(newPOI);
            }

            // 캐시에 최신 데이터 저장
            poiDataCache[newPOI.id] = newPOI;
        }

    }

    // 해당되는 Tag의 POI 위치 업데이트하기
    void UpdatePOI(POIData poi)
    {
        if (poiObjects.ContainsKey(poi.id))
        {
            poiObjects[poi.id].transform.position = new Vector3(poi.x, poi.y, poi.z);
            Debug.Log($"POI {poi.name} 위치 업데이트 → ({poi.x}, {poi.y}, {poi.z})");
        }
    }

    // POI 새로 생성하기
    void CreatePOI(POIData poi)
    {
        GameObject newPOI = Instantiate(poiPrefab, new Vector3(poi.x, poi.y, poi.z), Quaternion.identity, parentObject);
        newPOI.name = poi.name;
        newPOI.tag = poi.tag; // 생성 시 태그 설정
        poiObjects[poi.id] = newPOI;

        Debug.Log($"새로운 POI 오브젝트 생성: {poi.name}");
        Debug.Log(JsonConvert.SerializeObject(poi));

        if (!string.IsNullOrEmpty(poi.event_type))
            {
                if (poi.event_type == "경고")
                {
                    Debug.Log($"{poi.name} 경고 이벤트 할당되었음");
                }
                else if (poi.event_type == "알림")
                {
                    Debug.Log($"{poi.name} 알림 이벤트 할당되었음");
                }
            }
    }

    // POI Tag 업데이트
    void UpdatePOITag(POIData poi)
    {
        if (poiObjects.ContainsKey(poi.id))
        {
            poiObjects[poi.id].tag = poi.tag; // Unity 오브젝트 태그 변경
            Debug.Log($"POI {poi.name} 태그 업데이트 → {poi.tag}");
        }
    }
}
