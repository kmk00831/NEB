using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PositionAPI : MonoBehaviour
{
    private string baseUrl = "http://localhost:5000/positions";

    void Start()
    {
        Debug.Log("Start() 실행 - 좌표 전송 및 매핑 시작");
        InvokeRepeating(nameof(SendPositionToServer), 0f, 10f);
    }

    void SendPositionToServer()
    {
        // 최신 Position 오브젝트 탐색
        Transform latestPosition = FindLatestPositionByHierarchyOrder();
        if (latestPosition == null)
        {
            Debug.LogWarning("실시간 위치 오브젝트를 찾을 수 없습니다.");
            return;
        }

        Vector3 unityPos = latestPosition.position;
        Vector2 mapCoord = UnityToMap(unityPos.x, unityPos.z);

        Debug.Log($"[유니티 → 도면] ({unityPos.x:F2}, {unityPos.z:F2}) → (도면 X: {mapCoord.x:F0}, Y: {mapCoord.y:F0})");

        StartCoroutine(SavePosition(1, unityPos.x, unityPos.y, unityPos.z));
    }

    // 가장 최근에 생성된 Position 오브젝트 찾기
    Transform FindLatestPositionObject()
    {
        Transform latest = null;
        int latestId = int.MinValue;

        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Position_1_"))
            {
                int id = child.GetInstanceID(); 
                if (id > latestId)
                {
                    latest = child;
                    latestId = id;
                }
            }
        }
        Debug.Log($"가장 최신 위치 오브젝트: {latest.name}, 좌표: {latest.position}");
        Debug.Log($"latestId: {latestId}");

        return latest;
    }

    // 가장 마지막 자식 반환하기
    Transform FindLatestPositionByHierarchyOrder()
    {
        Transform parent = GameObject.Find("GameObject").transform;

        if (parent.childCount == 0)
            return null;

        return parent.GetChild(parent.childCount - 1); 
    }

    Vector2 UnityToMap(float unityX, float unityZ)
    {
        float dx = unityX - 1548f;
        float dz = unityZ - 765f;

        float mapX = -0.0178f * dx + 1.0669f * dz + 235f;
        float mapY =  0.8545f * dx + 0.0096f * dz + 150f;

        return new Vector2(Mathf.Round(mapX), Mathf.Round(mapY));
    }

    public IEnumerator SavePosition(int userId, float x, float y, float z)
    {
        string url = $"{baseUrl}/save";
        PositionData data = new PositionData { user_id = userId, x = x, y = y, z = z };
        string jsonData = JsonUtility.ToJson(data);

        Debug.Log("Unity → 서버로 좌표 전송");
        Debug.Log($"JSON Data: {jsonData}");

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("좌표 저장 성공: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("좌표 저장 실패: " + request.error);
        }
    }
}

[System.Serializable]
public class PositionData
{
    public int user_id;
    public float x;
    public float y;
    public float z;
}
