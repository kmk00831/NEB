using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.AI; // NavMesh 관련 기능을 사용하기 위해 필요합니다.



public class SetPositionNeb : MonoBehaviour
{
    // Public variables (Inspector에서 설정)
    public Transform CenterPoint;
    public GameObject positionPrefab;
    public float rotationSpeed = 2.0f;
    public float cameraDistance = 0.0f;




    // Private state variables
    private bool isFirstExecution = true;
    private Dictionary<int, List<GameObject>> positionHistory = new Dictionary<int, List<GameObject>>();
    private int currentFloor = 1;
    private int newFloor = 1;
    private Dictionary<int, GameObject> floorDictionary;
    private const int maxHistoryCount = 3;
    private ShowFloor floorDisplay;

    // 파일에서 읽은 데이터 (자동 테스트 용)
    private List<List<float>> allValues = new List<List<float>>();
    private string filePath = "./Assets/SampleData/position_history.txt";

    // 초기 좌표 값
    private float currentX = 260f;
    private float currentZ = 432f;
    // 자이로 회전값 저장
    public float lastRotationY = 0f;

    // MobileCameraController에서 호출할 함수
    private bool positionUpdated = false;
    // 0418 김명권
    public int FloorController = 8;
    private int PreFloorController = 0;
    private Vector3 rotation_position; // 회전 방향을 위한 벡터
    private List<GameObject> stairCeilingObjects = new List<GameObject>();
    private float floorY;
    private string FloorChange;
    private float NextFloorY;
    private string NextFloorChange;
    private int NextFloor;
    public GameObject objectToSpawn;
    public float maxDistance = 10.0f; // 주변 탐색 거리
    // 객체 참조
    public Setup setup;

    // 동일 위치에서 오브젝트가 계속 생기는 것을 방지
    Dictionary<string, Vector2> userPositions = new Dictionary<string, Vector2>();
    
    public bool HasPositionBeenUpdated()
    {
        return positionUpdated;
    }


    private void Start()
    {
        setup = FindObjectOfType<Setup>();
        if (positionPrefab == null)
        {
            Debug.LogError("Position object is not assigned in the Inspector!");
            return;
        }

        // 층(GameObject)들을 Dictionary에 등록
        floorDictionary = new Dictionary<int, GameObject>
        {
            { -1, GameObject.Find("B1F") },
            { 1, GameObject.Find("1F") },
            { 2, GameObject.Find("2F") },
            { 3, GameObject.Find("3F") },
            { 4, GameObject.Find("4F") },
            { 5, GameObject.Find("5F") },
            { 6, GameObject.Find("6F") },
            { 7, GameObject.Find("7F") },
            { 8, GameObject.Find("8F") }
        };

        // Player를 찾아서 투명 처리
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            Renderer renderer = player.GetComponent<Renderer>();
            if (renderer != null)
            {
                SetObjectTransparent(renderer);
                Debug.Log("Player made completely transparent");
            }
            else
            {
                Debug.LogWarning("Player does not have a Renderer component");
            }
        }
        else
        {
            Debug.LogWarning("Could not find Player object in Start");
        }
        floorDisplay = FindObjectOfType<ShowFloor>();
        // 최초에 모든 stairCeiling 오브젝트를 찾아서 리스트에 저장
        stairCeilingObjects = GameObject.FindObjectsOfType<GameObject>()
            .Where(obj => obj.name.ToLower().Contains("stairceiling"))
            .ToList();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (FloorController < setup.maxFloor)
            {
                FloorController++;
                if (FloorController == 0) { FloorController = 1; }
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (FloorController > setup.minFloor)
            {
                FloorController--;
                if (FloorController == 0) { FloorController = -1; }
            }
        }
        if (FloorController != PreFloorController)
        {
            ClearUpperFloors(FloorController);
            HideUserObjectsAboveHeight(FloorController);
        }
        PreFloorController = FloorController;
    }

    private void SetObjectTransparent(Renderer renderer)
    {
        Color color = renderer.material.color;
        color.a = 0f;
        renderer.material.color = color;

        renderer.material.SetFloat("_Mode", 3); // Transparent 모드
        renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        renderer.material.SetInt("_ZWrite", 0);
        renderer.material.DisableKeyword("_ALPHATEST_ON");
        renderer.material.EnableKeyword("_ALPHABLEND_ON");
        renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        renderer.material.renderQueue = 3000;
    }

    float preX;
    float preZ;
    public void ShowMyPosition(string result, int uid, int statereal)
    {
        positionUpdated = true;

        if (positionPrefab == null)
        {
            Debug.LogError("Position object is not assigned!");
            return;
        }

        string[] parts = result.Split('\t');
        if (parts.Length < 4)
        {
            Debug.LogError("Invalid result format. Expected format: x\tz\tfloor");
            return;
        }

        float x = float.Parse(parts[1]);
        float z = float.Parse(parts[0]);
        int newFloor = int.Parse(parts[2]);
        float res_distance = float.Parse(parts[3]);
        cameraDistance = res_distance;
        
        FloorChange = newFloor.ToString();
        if(newFloor < 0){FloorChange = FloorChange.Replace("-", "B");} // -를 B로 대치
        string floorYName = "Floor_" + FloorChange + "F";
        // 이름에 "Floor값이 모두 포함된 오브젝트를 찾습니다.
        GameObject floorObject = GameObject.FindObjectsOfType<GameObject>()
            .FirstOrDefault(obj => obj.name.Contains(floorYName));
        // 오브젝트가 존재하는지 확인합니다.
        if (floorObject != null)
        {
            // 바닥 오브젝트의 y 좌표
            floorY = floorObject.transform.position.y;   
            Debug.Log($"오브젝트 높이: {floorY}");   
        }

        // 비교 대상 오브젝트의 이름을 만듭니다.
        string targetObjectName = $"USER_{uid}";

        // 씬에서 해당 이름의 오브젝트를 찾습니다.
        GameObject targetUserObject = GameObject.Find(targetObjectName);

        // 만약 해당 오브젝트를 찾았다면 (null이 아니면)
        if (targetUserObject != null)
        {
            // 찾은 오브젝트의 현재 위치를 가져옵니다.
            Vector3 targetPosition = targetUserObject.transform.position;

            // 찾은 오브젝트의 x, z 위치를 반올림합니다.
            float roundedTargetX = Mathf.Round(targetPosition.x) - 1351;
            float roundedTargetZ = Mathf.Round(targetPosition.z) - 603;

            Vector2 curPos = new Vector2(x, z);
            // 찾은 오브젝트의 현재 x, z 위치와 새로 받은 x, z 값을 비교합니다.
            if (userPositions.TryGetValue($"USER_{uid}", out Vector2 pos))
            {
                if (Vector2.Distance(pos, curPos) < 0.01f)  // 거의 같은 위치면
                {
                    return; // 위치 변화 없음 → 처리 생략
                }
            }
        }

        // 층이 바뀌든 안 바뀌든 무조건 UI 갱신
        if (floorDisplay != null)
        {
            floorDisplay.UpdateFloorDisplay(currentFloor);
        }

        GameObject player = GameObject.Find("Player");

        if (player == null)
        {
            Debug.LogError("Player object not found!");
            return;
        }

        Vector3 worldPos = player.transform.parent.TransformPoint(new Vector3(x, 0, z));
        // 기존 플레이어의 y 좌표 가져오기
        float lastPlayerY = player.transform.position.y;
        // 최근 플레이어 y와 가장 가까운 y값을 Ground에서 탐색
        float pos_y = floorY + 6.0f;

        if (isFirstExecution)
        {
            Debug.Log("Setting initial CenterPoint position...");
            // Editor에서 설정한 CenterPoint의 초기 위치 유지
            isFirstExecution = false;
        }
 
        // 플레이어 위치 업데이트
        player.transform.localPosition = new Vector3(x, pos_y, z);
        Vector3 afterPosition = new Vector3(x, pos_y, z);

        // // 마커 오브젝트 생성
        GameObject newPosition = Instantiate(positionPrefab);
        newPosition.layer = LayerMask.NameToLayer("UserClickEvent"); // "UserClickEvent" 레이어로 설정 0418 김명권
        newPosition.transform.SetParent(GameObject.Find("GameObject").transform, false);
        newPosition.transform.localPosition = new Vector3(x,pos_y, z); // 최종 로컬 위치 설정 (Y는 NavMesh에서 찾은 값 사용)


        // Vector3 NavPosition = new Vector3(x + 1351, floorY, z + 603);
        // NavMeshHit hit;
        // if (NavMesh.SamplePosition(NavPosition, out hit, 50.0f, NavMesh.AllAreas)) //이거 한 50으로 해도 무방할 것 같은데
        // {
        //     NavMeshAgent agent = newPosition.GetComponent<NavMeshAgent>();
        //     if (agent != null)
        //     {
        //         if (!agent.Warp(hit.position))
        //         {
        //             Debug.LogWarning($"Failed to warp agent {newPosition.name} to {hit.position}");
        //         }
        //     }

        // newPosition.transform.localPosition = new Vector3(x, hit.position.y, z); // 최종 로컬 위치 설정 (Y는 NavMesh에서 찾은 값 사용)
        newPosition.transform.localPosition = new Vector3(x, pos_y, z); // 최종 로컬 위치 설정 (Y는 NavMesh에서 찾은 값 사용)
        newPosition.transform.localScale = Vector3.one * 7.5f;
        newPosition.name = $"USER_{uid}";

        GameObject targetObject = GameObject.Find($"USER_{uid}");
        // 오브젝트가 존재하는지 확인
        if (targetObject != null)
        {
            // 오브젝트의 위치 얻기
            rotation_position = targetObject.transform.position;
            Vector3 direction = afterPosition - rotation_position;
            // 방향 벡터가 0이 아닐 경우에만 회전 적용
            if (direction != Vector3.zero)
            {
                // Y축 회전 각도 계산
                lastRotationY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

                // 회전 값 적용ㄴ
                newPosition.transform.rotation = Quaternion.Euler(0, lastRotationY, 0);
            }
        }

        // SphereCollider 활성화 클릭을 위해서
        SphereCollider collider = newPosition.GetComponent<SphereCollider>();
        if (!collider.enabled)
        {
            collider.enabled = true;
            collider.isTrigger = true;
        }

        newPosition.transform.rotation = Quaternion.Euler(0, lastRotationY, 0); //김명권

        newPosition.transform.localPosition = new Vector3(x, pos_y, z);
        newPosition.transform.localScale = Vector3.one * 7.5f;
        newPosition.name = $"USER_{uid}";

        // 히스토리에 추가 및 최대 개수 제한 적용 (가장 최근 자식 객체만 유지)
        if (!positionHistory.ContainsKey(newFloor))
        {
            positionHistory[newFloor] = new List<GameObject>();
        }
        // 새로운 객체 추가 (자식 객체 유지)
        positionHistory[newFloor].Add(newPosition);
        UpdateColors(statereal, uid);

        if(newFloor > FloorController){
            Renderer renderer = newPosition.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }
        }
        userPositions[$"USER_{uid}"] = new Vector2(x, z);
        // }
        // else
        // {
        //     // NavMesh 표면을 찾지 못했을 경우의 처리 (오브젝트 삭제, 에이전트 비활성화 등)
        //     Destroy(newPosition); // 예시: 생성 실패 시 오브젝트 삭제
        //     return; // 함수 종료 또는 다른 오류 처리
        // }
    }

    private float GetZPosition(int floor)
    {
        float pos_y;
        switch (floor)
        {
            case -1: pos_y = -158.4f; break;
            case 1: pos_y = 1.0f; break;
            case 2: pos_y = 113.6f; break;
            case 3: pos_y = 218.8f; break;
            case 4: pos_y = 322.1f; break;
            default:
                // Debug.LogWarning($"Unexpected floor value: {floor}, defaulting to 0.0f");
                pos_y = 0.0f;
                break;
        }
        // Debug.Log($"Calculated pos_y for floor {floor}: {pos_y}");
        return pos_y;
    }

    /// <summary>
    /// 지정한 층의 마커들을 삭제합니다.
    /// </summary>
    private void ClearHistory(int floor)
    {
        if (positionHistory.ContainsKey(floor))
        {
            foreach (GameObject obj in positionHistory[floor])
            {
                //Destroy(obj);
                if (obj != null)
                {
                    Destroy(obj); // 즉시 삭제
                }
            }
            positionHistory[floor].Clear();
        }
    }

    /// <summary>
    /// 현재 층보다 높은 층은 비활성화하고, 새로운 층부터 -1층까지 활성화합니다.
    /// </summary>
    // private void ClearUpperFloors(int newFloor)
    // {
    //     Debug.Log($"Clearing floors above {newFloor}");

    //     // 모든 층 비활성화
    //     foreach (var floor in floorDictionary.Values)
    //     {
    //         if (floor != null)
    //         {
    //             floor.SetActive(false);
    //         }
    //     }

    //     // newFloor부터 -1층까지 활성화
    //     for (int i = newFloor; i >= -1; i--)
    //     {
    //         if (floorDictionary.TryGetValue(i, out GameObject activeFloor) && activeFloor != null)
    //         {
    //             activeFloor.SetActive(true);
    //             // Debug.Log($"Activated Floor: {i}");
    //         }
    //     }
    // }

    public GameObject Modeling;  // 인스펙터에서 할당

    public void ClearUpperFloors(int currentFloor)
    {
        if ( Modeling == null)
        {
             Modeling = GameObject.Find(setup.ModelingName);

            if (Modeling == null)
            {
                Debug.LogError("GameObject.Find(\" Modeling\") 실패: Hierarchy에 ' Modeling'라는 이름의 오브젝트가 없습니다.");
            }
        }

        foreach (Transform child in  Modeling.transform)
        {
            int childFloor = GetFloorFromObjectName(child.name);

            if (childFloor <= currentFloor)
                child.gameObject.SetActive(true);
            else
                child.gameObject.SetActive(false);
        }
    }

    // 오브젝트 이름에서 층 정보를 추출하는 부분
    private int GetFloorFromObjectName(string name)
    {
        name = name.ToUpper().Trim();

        // "B1F", "B2F" → -1, -2
        if (name.StartsWith("B") && name.EndsWith("F"))
        {
            string middle = name.Substring(1, name.Length - 2); // B1F → "1"
            if (int.TryParse(middle, out int basement))
                return -basement;
        }

        // "1F", "2F" → 1, 2
        if (name.EndsWith("F") && char.IsDigit(name[0]))
        {
            string number = name.Substring(0, name.Length - 1); // "2F" → "2"
            if (int.TryParse(number, out int floor))
                return floor;
        }

        Debug.LogWarning($"❓ 층 이름 파싱 실패: {name}");
        return int.MinValue;
    }
    /// ///////////////////////////////////////////////////////////////////////////

    private void UpdatePositionColors(int floor)
    {
        if (!positionHistory.ContainsKey(floor))
            return;

        List<GameObject> markers = positionHistory[floor];
        int count = markers.Count;
        for (int i = 0; i < count; i++)
        {
            Renderer renderer = markers[i].GetComponent<Renderer>();
            if (renderer != null)
            {
                Color color = renderer.material.color;
                color.a = Mathf.Lerp(0.0f, 1.0f, (float)(i + 1) / count);
                renderer.material.color = color;
            }
        }
    }

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

        string targetObjectName = $"USER_{uid}";
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
                        Color currentColor = instanceMaterial.GetColor("_BaseColor");
                        newColor.a = currentColor.a;
                        instanceMaterial.SetColor("_BaseColor", newColor);
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning($"No objects found with name pattern: {targetObjectName}");
        }
    }

    public void UpdatePlayerColorByDistance()
    {
        GameObject player = GameObject.Find("Player");
        if (player == null) return;

        Renderer renderer = player.GetComponent<Renderer>();
        if (renderer == null) return;

        //Color newColor;
        Color newColor = Color.red; // 기본값으로 초기화

        if (cameraDistance >= 0f && cameraDistance < 3f)
            newColor = new Color(1.00f, 0.00f, 0.00f, 1f); // 완전 빨강
        else if (cameraDistance >= 3f && cameraDistance < 6.5f)
            newColor = new Color(1.00f, 0.24f, 0.12f, 1f); // 주황+빨강 중간
        else if (cameraDistance >= 6.5f && cameraDistance < 10f)
            newColor = new Color(1.00f, 0.47f, 0.08f, 1f); // 진한 주황
        else if (cameraDistance >= 10f)
            newColor = new Color(1.00f, 0.67f, 0.20f, 1f); // 밝은 주황


        renderer.material.SetColor("_BaseColor", newColor);
    }

    void HideUserObjectsAboveHeight(int FloorController)
    {
        NextFloor = FloorController + 1;
        if(NextFloor == 0){NextFloor = 1;} // 0층을 1층으로 대치
        NextFloorChange = NextFloor.ToString();
        if(NextFloor < 0){NextFloorChange = NextFloorChange.Replace("-", "B");} // -는 B로 대치
        string floorObjectName = "Floor_" + NextFloorChange + "F";

        // GameObject NextfloorObject = GameObject.FindObjectsOfType<GameObject>()
        //     .FirstOrDefault(obj => obj.name.Contains("Floor") && obj.name.Contains(NextFloorChange) && !obj.name.Contains("B1"));

        GameObject parentINB = GameObject.Find(setup.ModelingName); // **
        // 부모 오브젝트를 찾았는지 확인
        GameObject NextfloorObject = null; // 미리 null로 초기화
        NextfloorObject = parentINB.GetComponentsInChildren<Transform>(true)
        .Select(t => t.gameObject)
        .FirstOrDefault(obj =>
            obj.name.Contains(floorObjectName));

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
            if (obj.name.StartsWith("USER_"))
            {
                // 오브젝트의 Y 위치가 기준보다 높은지 확인
                if (obj.transform.position.y > NextFloorY)
                {
                    // 오브젝트의 Renderer 컴포넌트를 가져옴
                    Renderer renderer = obj.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.enabled = false; // 렌더러 비활성화 (안 보이게 함)
                    }
                }
            }
        }
    }
}
