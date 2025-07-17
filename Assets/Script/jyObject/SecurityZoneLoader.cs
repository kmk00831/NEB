using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class SecurityZoneData
{
    public int id;
    public string name;
    public float point1_x;
    public float point1_y;
    public float point2_x;
    public float point2_y;
    public float point3_x;
    public float point3_y;
    public float point4_x;
    public float point4_y;
    public string floor;

}

public class SecurityZoneLoader : MonoBehaviour
{
    private string apiUrl = "http://localhost:3001/security/list";
    private List<SecurityZoneBounds> zoneBoundsList = new List<SecurityZoneBounds>();
    private Transform player;
    public Image securityImg;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        StartCoroutine(LoadZonesFromServer());
    }

    void Update()
    {
        if (player == null) return;

        Vector3 playerPos = player.position;

        bool isInsideAnyZone = false;

        foreach (var zone in zoneBoundsList)
        {
            if (!zone.hasEntered && zone.Contains(playerPos))
            {
                Debug.Log($"{zone.name} 안전구역 진입!");
                zone.hasEntered = true;
            }
            else if (zone.hasEntered && !zone.Contains(playerPos))
            {
                Debug.Log($"{zone.name} 안전구역 이탈");
                zone.hasEntered = false;
            }

            if (zone.Contains(playerPos))
            {
                isInsideAnyZone = true;
            }
        }

        if (securityImg != null)
        {
            securityImg.enabled = isInsideAnyZone; // 있으면 보이게, 없으면 숨김
        }
    }

    IEnumerator LoadZonesFromServer()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            SecurityZoneData[] zones = JsonHelper.FromJsonArray<SecurityZoneData>(json);

            foreach (var zone in zones)
            {
                Vector3 p1 = MapToUnityVector(zone.point1_x, zone.point1_y);
                Vector3 p3 = MapToUnityVector(zone.point3_x, zone.point3_y);

                SecurityZoneBounds bounds = new SecurityZoneBounds
                {
                    name = zone.name,
                    min = Vector3.Min(p1, p3),
                    max = Vector3.Max(p1, p3),
                    hasEntered = false
                };

                zoneBoundsList.Add(bounds);

                CreateZoneVisual(zone.name, p1, p3);
            }
        }
        else
        {
            Debug.LogError("보안구역 불러오기 실패: " + request.error);
        }
    }

    void CreateZoneVisual(string name, Vector3 p1, Vector3 p3)
    {
        Vector3 center = (p1 + p3) / 2;
        Vector3 size = new Vector3(
            Mathf.Abs(p3.x - p1.x),
            5f,
            Mathf.Abs(p3.z - p1.z)
        );

        GameObject zoneObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        zoneObj.name = name;
        zoneObj.transform.position = center;
        zoneObj.transform.localScale = size;
        zoneObj.GetComponent<Renderer>().material.color = new Color(1f, 0f, 0f, 0.3f);

        BoxCollider collider = zoneObj.GetComponent<BoxCollider>();
        collider.isTrigger = true;
    }

    Vector2 MapToUnity(float mapX, float mapY)
{
    float unityX = 0.0008877f * mapX + 1.143877f * mapY + 1367.8252f;
    float unityZ = 0.9363842f * mapX + 0.0140422f * mapY + 544.2602f;

    return new Vector2(unityX, unityZ);
}


    Vector3 MapToUnityVector(float mapX, float mapY)
    {
        Vector2 pos = MapToUnity(mapX, mapY);
        return new Vector3(pos.x, 0f, pos.y);
    }
}

public class SecurityZoneBounds
{
    public string name;
    public Vector3 min;
    public Vector3 max;
    public bool hasEntered;

    public bool Contains(Vector3 position)
    {
        return position.x >= min.x && position.x <= max.x &&
               position.z >= min.z && position.z <= max.z;
    }
}

public static class JsonHelper
{
    public static T[] FromJsonArray<T>(string json)
    {
        string wrappedJson = "{\"items\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrappedJson);
        return wrapper.items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}
