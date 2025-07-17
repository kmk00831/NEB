using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UserPositionUpdate : MonoBehaviour
{
    // 참조
    public Setup setup;
    public SetPositionNeb setpositionneb;
    private DataFetcher fetcher;
    // ✅ 생성된 유저 오브젝트 추적용 딕셔너리
    private Dictionary<string, GameObject> activeUsers = new();
    // 오브젝트의 자연스러운 움직임을 위해 각 유저의 속도 벡터 저장
    private Dictionary<int, Vector3> moveVelocities = new();
     // 유저별 자취 오브젝트 큐
    private Dictionary<string, Queue<GameObject>> userTrails = new();
    private int maxTrailCount = 20; // 최대 자취 개수
    // 유저별 마지막 위치 저장용
    private Dictionary<string, Vector3> lastTrailPositions = new();

    void Start()
    {
        setup = FindObjectOfType<Setup>();
        setpositionneb = FindObjectOfType<SetPositionNeb>();
        fetcher = FindObjectOfType<DataFetcher>();
    }

    void Update()
    {
        Debug.DrawLine(Vector3.zero, new Vector3(2000, 0, 0), Color.red, 10f);
        Debug.DrawLine(Vector3.zero, new Vector3(0, 0, 2000), Color.blue, 10f);
        UserObjectManager();
        HideUserObjectsAboveHeight(setpositionneb.FloorController);
    }

    void UserObjectManager()
    {
        HashSet<string> currentActiveUIDs = new();
        // UserData[] users = Communication.CurrentUsers;
        if (fetcher != null && fetcher.latestData != null)
        {
            foreach (var user in fetcher.latestData)
            {
                if (user.userStatus == "Active")
                {
                    Debug.Log($"🟢 {user.userId}: ({user.userX}, {user.userY}) 층:{user.userFloor}");
                    string uid = user.userId;
                    currentActiveUIDs.Add(uid);


                    float x = user.userX;
                    float z = user.userY;
                    int pos_y = FloorYPosition(user.userFloor) + 6;
                    Vector3 targetLocalPos = new Vector3(x, pos_y, z); // ✅ 오프셋 제거됨

                    // 🔄 위치 같으면 패스
                    if (lastTrailPositions.TryGetValue(uid, out Vector3 lastPos))
                    {
                        if (Vector3.Distance(lastPos, targetLocalPos) < 0.01f)
                            continue;
                    }
                    lastTrailPositions[uid] = targetLocalPos;

                    if (!userTrails.ContainsKey(uid))
                    userTrails[uid] = new Queue<GameObject>();

                    GameObject trail = Instantiate(setup.CSprefab);
                    trail.name = $"{uid}";
                    trail.transform.SetParent(GameObject.Find("GameObject").transform, false);
                    trail.transform.localPosition = targetLocalPos;
                    trail.transform.localScale = Vector3.one * 7.5f;

                    var sensorFetcher = FindObjectOfType<SensorFetcher>();
                    if (sensorFetcher != null)
                    {
                        sensorFetcher.RequestSensorForUser(uid, (userStateReal) =>
                        {
                            if (userStateReal.HasValue)
                                Debug.Log($"✅ [{uid}] userStateReal: {userStateReal.Value}");
                            else
                                Debug.LogWarning($"⚠️ [{uid}] userStateReal 데이터를 가져올 수 없습니다.");
                        });
                    }

                    userTrails[uid].Enqueue(trail);

                    if (userTrails[uid].Count > maxTrailCount)
                    {
                        GameObject oldest = userTrails[uid].Dequeue();
                        Destroy(oldest);
                    }
                }
            }

            // 🔥 삭제 처리
            List<string> toRemove = new();
            foreach (var kvp in activeUsers)
            {
                if (!currentActiveUIDs.Contains(kvp.Key))
                {
                    Destroy(kvp.Value);
                    toRemove.Add(kvp.Key);
                }
            }

            foreach (string uid in toRemove)
            {
                activeUsers.Remove(uid);
            }
        }
        else
        {
            Debug.Log("아직 사용자 데이터가 로딩되지 않았습니다.");
            foreach (var kvp in activeUsers)
            {
                Destroy(kvp.Value);
            }
            activeUsers.Clear();
        }
    }

    // 층 바닥 위치 정보를 가져와 오브젝트에 반영
    int FloorYPosition(int newFloor)
{
    string code = newFloor < 0 ? "B" + Mathf.Abs(newFloor) : newFloor.ToString(); // e.g., "B1"
    string floorGroupName = code + "F"; // e.g., "B1F"
    string containerName = $"Floor_{code}F"; // e.g., "Floor_B1"
    string targetName = $"{code}_Floor";     // e.g., "B1_Floor"

    var modeling = GameObject.Find("Modeling");
    if (modeling == null)
    {
        Debug.LogError("❌ 'Modeling' 오브젝트를 찾을 수 없습니다.");
        return 0;
    }

    var floorGroup = modeling.transform.Find(floorGroupName);
    if (floorGroup == null)
    {
        Debug.LogWarning($"⚠️ '{floorGroupName}'을 찾을 수 없습니다.");
        return 0;
    }

    var floors = floorGroup.Find("Floors");
    if (floors == null)
    {
        Debug.LogWarning($"⚠️ 'Floors' 오브젝트를 찾을 수 없습니다.");
        return 0;
    }

    var floorContainer = floors.Find(containerName);
    if (floorContainer == null)
    {
        Debug.LogWarning($"⚠️ '{containerName}' 오브젝트를 찾을 수 없습니다.");
        return 0;
    }

    var floorObj = floorContainer.Find(targetName);
    if (floorObj == null)
    {
        Debug.LogWarning($"⚠️ '{targetName}' 오브젝트를 찾을 수 없습니다.");
        return 0;
    }

    Debug.Log($"✅ {targetName} 위치: {floorObj.position.y} (로컬)");
    if(newFloor == 1)
        return 0;
    return Mathf.RoundToInt(floorObj.position.y);
}



    private void test22(int? state, GameObject trail)
    {
        Color newColor;
        switch (state)
        {
            case 0: newColor = Color.red; break; // Normals
            case 1: newColor = Color.yellow; break; //swing
            case 2: newColor = Color.green; break; //inpocket
            case 3: newColor = Color.magenta; break; //phone call
            case 4: newColor = Color.black; break; //state judging
            case 22: newColor = Color.cyan; break; //자켓 주머니
            case 32: newColor = Color.blue; break; //바지 뒷 주머니
            default: newColor = Color.gray; break; // Unknown State
        }

        Renderer renderer = trail.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = renderer.material;
            mat.color = newColor;
        }
    }
    // 파지 상태 색깔
    private void UpdateColors(int state, int uid)
    {
        Color newColor;
        switch (state)
        {
            case 0: newColor = Color.red; break; // Normals
            case 1: newColor = Color.yellow; break; //swing
            case 2: newColor = Color.green; break; //inpocket
            case 3: newColor = Color.magenta; break; //phone call
            case 4: newColor = Color.black; break; //state judging
            case 22: newColor = Color.cyan; break; //자켓 주머니
            case 32: newColor = Color.blue; break; //바지 뒷 주머니
            default: newColor = Color.gray; break; // Unknown State
        }

        string targetObjectName = $"uid";
        // 해당 이름 패턴을 가진 모든 오브젝트를 찾습니다.
        GameObject[] userObjects = GameObject.FindObjectsOfType<GameObject>()
            .Where(obj => obj.name == targetObjectName)
            .ToArray();

        if (userObjects.Length > 0)
        {
            foreach (GameObject userObject in userObjects)
            {
                Renderer[] renderers = userObject.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    if (renderer != null)
                    {
                        // 머티리얼을 복사하여 인스턴스화합니다.
                        Material instanceMaterial = renderer.material; // renderer.material은 머티리얼의 인스턴스를 반환합니다.

                        // 기존 알파값 유지
                        Color currentColor = instanceMaterial.GetColor("_Color");
                        newColor.a = currentColor.a;
                        instanceMaterial.SetColor("_Color", newColor);
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning($"No objects found with name pattern: {targetObjectName}");
        }
    }

    private float NextFloorY;
    private string NextFloorChange;
    private int NextFloor;
    void HideUserObjectsAboveHeight(int FloorController)
    {
        NextFloor = FloorController + 1;
        if(NextFloor == 0){NextFloor = 1;} // 0층을 1층으로 대치
        NextFloorChange = NextFloor.ToString();
        if(NextFloor < 0){NextFloorChange = NextFloorChange.Replace("-", "B");} // -는 B로 대치
        // string floorObjectName = "Floor_" + NextFloorChange + "F";
        string floorObjectName = NextFloorChange + "_Floor"; // 신공학관 전용
        GameObject parentINB = GameObject.Find(setup.ModelingName); // **
        // 부모 오브젝트를 찾았는지 확인
        GameObject NextfloorObject = null; // 미리 null로 초기화
        NextfloorObject = parentINB.GetComponentsInChildren<Transform>(true)
        .Select(t => t.gameObject)
        .FirstOrDefault(obj => obj.name == floorObjectName);

        // 오브젝트가 존재하는지 확인합니다.
        if (NextfloorObject != null)
        {
            NextFloorY = NextfloorObject.transform.position.y;     
        }

        // 모든 오브젝트를 가져옴
        // GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        GameObject parentGameObject = GameObject.Find("GameObject");

        // 부모 오브젝트를 찾았는지 확인하고 자식들을 가져옴
        GameObject[] allObjects = null; // 미리 null로 초기화
        allObjects = parentGameObject.GetComponentsInChildren<Transform>(true)
        .Select(t => t.gameObject) // Transform 배열을 GameObject 배열로 변환
        .ToArray(); // List가 아니라 배열로 필요하면 ToArray()

        foreach (GameObject obj in allObjects)
        {
            // 오브젝트 이름이 "USER_"로 시작하는지 확인
            // if (obj.name.StartsWith("USER_"))
            // {
                // 오브젝트의 Y 위치가 기준보다 높은지 확인
                if (obj.transform.localPosition.y > NextFloorY)
                {
                    // 오브젝트의 Renderer 컴포넌트를 가져옴
                    Renderer renderer = obj.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.enabled = false; // 렌더러 비활성화 (안 보이게 함)
                    }
                }
            // }
        }
    }
}