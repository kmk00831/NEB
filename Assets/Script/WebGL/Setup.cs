using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class Setup : MonoBehaviour
{
    // 카메라 오브젝트
    private GameObject cam1GameObject;
    private GameObject cam2GameObject;
    private GameObject cam3GameObject;
    private Camera overviewCamera;
    private Camera observerCamera;
    private Camera userCamera;
    private Transform overviewCameraTransform;
    private Transform observerCameraTransform;
    private Transform userCameraTransform;

    // 큐브 오브젝트
    private GameObject myCube;
    private Transform myCubeTransform;

    // 텍스트 박스 오브젝트
    private GameObject UserLogObject;
    private GameObject UserInfoText;
    private TextMeshProUGUI UserLogText;

    // 건물 층 정보
    public int maxFloor;
    public int minFloor;
    private Vector3 lowestFloorPosition = Vector3.zero;
    private Vector3 highestFloorPosition = Vector3.zero;

    //레이어 설정
    public LayerMask targetLayerMask;

    // 초기화가 끝난 후 로드할 실제 게임 씬 이름
    public string SceneName = "YourMainMenuScene"; // <-- 여기에 실제 시작할 씬 이름 넣어줘!
    public string ModelingName = "YourModelingName";

    // 플레이어 프리팹
    public GameObject CSprefab;
    public GameObject CSparent
    ;
    // 게임이 시작될 때 가장 먼저 실행되는 함수!
    void Awake()
    {
        // 실행 함수
        CreateCameras();
        CreateCube();
        CreateEventSystem();
        FindAndPrintFloorPositionsWithFloorsChild();
        CreateControlSystemPlayerObject();
        AddComponent();
        Debug.Log("초기화 작업 완료! 다음 씬 로드 준비!");
        // --- 초기화 작업 끝 ---
    }

    void Start() // 또는 다른 적절한 함수 (Awake, 특정 이벤트 발생 시 등)
    {
        Debug.Log("▶️ DirectionArrow 오브젝트 비활성화 시도...");

        // 1. 먼저 'Player' 오브젝트를 찾습니다.
        GameObject playerObject = GameObject.Find("Player");

        // 'Player' 오브젝트를 찾았는지 확인!
        if (playerObject != null)
        {
            Debug.Log("✅ 'Player' 오브젝트 찾음!");

            // 2. 'Player' 오브젝트의 자식들 중에서 'DirectionArrow'라는 이름의 오브젝트를 찾습니다.
            Transform directionArrowTransform = FindPlayerChildByName(playerObject.transform, "DirectionArrow");

            // 'DirectionArrow' 오브젝트를 찾았는지 확인!
            if (directionArrowTransform != null)
            {
                Debug.Log("✅ 'DirectionArrow' 오브젝트 찾음! 비활성화 시도.");
                
                // 3. 찾은 'DirectionArrow' 게임 오브젝트를 비활성화합니다.
                directionArrowTransform.gameObject.SetActive(false); // <--- 비활성화 코드! [[1]]

                Debug.Log("✅ 'DirectionArrow' 오브젝트 비활성화 완료!");
            }
            else
            {
                Debug.LogWarning("⚠️ 'Player' 오브젝트의 자식 중에서 'DirectionArrow' 오브젝트를 찾을 수 없습니다.");
                Debug.Log("ℹ️ 이름이나 계층 구조를 다시 확인해보세요.");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ 'Player' 오브젝트를 씬에서 찾을 수 없습니다.");
            Debug.Log("ℹ️ 이름이나 활성화 상태를 다시 확인해보세요.");
        }
    }

    // 부모 Transform 아래에서 특정 이름의 자식 Transform을 재귀적으로 찾는 헬퍼 함수
    Transform FindPlayerChildByName(Transform parent, string childName)
    {
        // 부모 Transform의 모든 자식 Transform 순회
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            // 자식의 이름이 찾으려는 이름과 같으면 반환
            if (child.name == childName)
            {
                return child;
            }

            Transform found = FindChildByName(child, childName);
            if (found != null)
            {
                return found;
            }
        }

        // 자식들 중에서 못 찾았으면 null 반환
        return null;
    }

    // 카메라 3개를 생성하는 함수 따로 만들기
    void CreateCameras()
    {
        void DeleteCameras()
        {
            Camera[] allCameras = Object.FindObjectsOfType<Camera>();

            foreach (Camera camera in allCameras)
            {
                Destroy(camera.gameObject);
            }

            Debug.Log("All cameras deleted!");

            // 필요하다면, Main Camera를 제외한 모든 카메라가 삭제되었는지 확인
            allCameras = Object.FindObjectsOfType<Camera>();
            if (allCameras.Length == 1 && allCameras[0].gameObject.name == "Main Camera")
            {
                Debug.Log("Main Camera만 남았습니다.");
            }
            else
            {
                Debug.LogWarning("Main Camera 외에 다른 카메라가 남아있을 수 있습니다.");
            }
        }

        // 모든 카메라 제거 /////////////////////////////////////////////////////////////
        DeleteCameras();
        Debug.Log("카메라 3개 생성 시작! 🔥");


        // 1번 카메라 //////////////////////////////////////////////////////////////////
        cam1GameObject = new GameObject("Camera(Overview)");
        overviewCamera = cam1GameObject.AddComponent<Camera>(); // 클래스 변수에 할당!
        overviewCamera.farClipPlane = 3000f; // Far Clip Plane을 3000으로 설정
        // --- AudioListener 컴포넌트 끄기 (null 체크 추가) ---
        AudioListener audioListener1 = cam1GameObject.GetComponent<AudioListener>();
        if (audioListener1 != null)
        {
            audioListener1.enabled = false; // AudioListener 비활성화!
            Debug.Log(cam1GameObject.name + "의 AudioListener 비활성화 완료.");
        }
        overviewCameraTransform = cam1GameObject.transform;
        overviewCameraTransform.eulerAngles = new Vector3(90f, 0f, 0f);

        Debug.Log(cam1GameObject.name + " 생성 및 설정 완료.");

        // 2번 카메라 //////////////////////////////////////////////////////////////////
        cam2GameObject = new GameObject("Camera(Observer)");
        observerCamera = cam2GameObject.AddComponent<Camera>(); // 클래스 변수에 할당!

        // --- AudioListener 컴포넌트 끄기 (null 체크 추가) ---
        AudioListener audioListener2 = cam2GameObject.GetComponent<AudioListener>();
        if (audioListener2 != null)
        {
            audioListener2.enabled = false; // AudioListener 비활성화!
            Debug.Log(cam2GameObject.name + "의 AudioListener 비활성화 완료.");
        }
        observerCameraTransform = cam2GameObject.transform;
        observerCameraTransform.eulerAngles = new Vector3(0f, 40f, 0f);

        Debug.Log(cam2GameObject.name + " 생성 및 설정 완료.");

        // 3번 카메라 //////////////////////////////////////////////////////////////////
        cam3GameObject = new GameObject("Camera(User)");
        userCamera = cam3GameObject.AddComponent<Camera>(); // 클래스 변수에 할당!

        // --- AudioListener 컴포넌트 끄기 (null 체크 추가) ---
        AudioListener audioListener3 = cam3GameObject.GetComponent<AudioListener>();
        if (audioListener3 != null)
        {
            audioListener3.enabled = false; // AudioListener 비활성화!
            Debug.Log(cam3GameObject.name + "의 AudioListener 비활성화 완료.");
        }
        userCameraTransform = cam3GameObject.transform;

        Debug.Log(cam3GameObject.name + " 생성 및 설정 완료.");

        Debug.Log("카메라 3개 생성 및 AudioListener 비활성화 완료! 👍");

        // --- 여기서 초기 카메라 상태 설정 ---
        // 클래스 변수를 사용해서 활성화/비활성화
        if (overviewCamera != null) overviewCamera.enabled = true;
        if (observerCamera != null) observerCamera.enabled = false;
        if (userCamera != null) userCamera.enabled = false;
        // ------------------------------------

        Debug.Log("초기 카메라 활성화/비활성화 상태 설정 완료.");
    }

    // 유틸리티 큐브 생성
    void CreateCube()
    {
        Debug.Log("3D 큐브 생성 시작! 🧱");

        // 큐브 생성
        myCube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // 생성된 큐브 게임 오브젝트의 이름 지정
        myCube.name = "Utility";

        // 생성된 큐브의 Transform 컴포넌트 가져오기 (위치, 회전, 스케일 조절할 때 필요)
        myCubeTransform = myCube.transform;

        // 큐브의 초기 위치 설정
        myCubeTransform.position = new Vector3(0, 0, 0);

        Debug.Log(myCube.name + " 큐브 생성 및 초기 위치 설정 완료! 좌표: " + myCubeTransform.position);

        // 큐브의 메쉬 랜더러 비활성화
        MeshRenderer cubeMeshRenderer = myCube.GetComponent<MeshRenderer>();
        if (cubeMeshRenderer != null)
        {
            cubeMeshRenderer.enabled = false;
        }
        else
        {
            Debug.LogError(myCube.name + "에서 MeshRenderer 컴포넌트를 찾을 수 없어요! 😭");
        }

        Debug.Log("3D 큐브 생성 완료! 👍");
    }

    // 이벤트 시스템 생성
    void CreateEventSystem()
    {
        Debug.Log("EventSystem 오브젝트 생성 시작! 🖱️");

        // 1. 기존에 씬에 EventSystem 오브젝트가 있는지 확인
        // 보통 씬에는 하나의 EventSystem만 있어야 해. 중복되면 문제될 수 있어.
        // FindObjectOfType<EventSystem>()는 현재 씬에서 EventSystem 컴포넌트가 붙어있는 첫 번째 오브젝트를 찾아줘.
        if (GameObject.FindObjectOfType<EventSystem>() == null)
        {
            // 2. EventSystem 오브젝트 생성
            GameObject eventSystemGameObject = new GameObject("EventSystem"); // 이름 지정

            // 3. EventSystem 컴포넌트 추가!
            eventSystemGameObject.AddComponent<EventSystem>();

            // 4. StandaloneInputModule 컴포넌트 추가! (UI 입력 처리에 필수적)
            eventSystemGameObject.AddComponent<StandaloneInputModule>();

            // 필요하다면 InputSystemUIInputModule 도 추가 (New Input System 사용 시)
            // using UnityEngine.InputSystem.UI; // InputSystem 관련 네임스페이스 필요
            // eventSystemGameObject.AddComponent<InputSystemUIInputModule>();


            // ✨✨ 필요하다면 생성된 오브젝트를 클래스 변수에 저장 ✨✨
            // eventSystemObject = eventSystemGameObject;
            // ----------------------------------------------------


            Debug.Log("EventSystem 오브젝트 생성 및 컴포넌트 부착 완료! 👍");
        }
        else
        {
            Debug.LogWarning("씬에 이미 EventSystem 오브젝트가 존재합니다. 새로 생성하지 않습니다.");
            EventSystem eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem != null)
            {
                eventSystem.enabled = true;
                Debug.Log("이벤트 시스템 활성화 완료!");
            }
        }
    }

    // 관제시스템 플레이어 오브젝트 추가
    void CreateControlSystemPlayerObject()
{
    // 1. CsObject 생성
    CSparent = new GameObject("CsObject");
    CSparent.transform.position = Vector3.zero; // ✅ 부모 위치 (0,0,0)

    // 2. player.prefab 로드
    CSprefab = Resources.Load<GameObject>("ControlSystemObject/player");
    if (CSprefab == null)
    {
        Debug.LogError("❌ prefab 로드 실패: Resources/ControlSystemObject/player 경로를 확인하세요.");
        return;
    }

    // 3. player 프리팹을 CsPlayer로 인스턴스화
    GameObject child = Instantiate(CSprefab);
    child.name = "CsPlayer";

    // 4. 부모-자식 설정 (기존 월드 좌표 유지)
    child.transform.position = new Vector3(192f, 0.6f, 324.6f); // ✅ 정확한 위치 지정
    child.transform.SetParent(CSparent.transform, worldPositionStays: true); // ✅ 월드 좌표 유지하면서 부모 지정

    Debug.Log("✅ CsPlayer (프리팹) 생성 및 위치 지정 완료");
}
    // 컴포넌트 추가
    void AddComponent()
    {
        // Overview 카메라에 컴포넌트 추가
        cam1GameObject.AddComponent<CameraOverView>();
        Debug.Log(cam1GameObject.name + "에 CameraOverView 스크립트 부착 완료.");
        // Observer 카메라에 컴포넌트 추가
        cam2GameObject.AddComponent<CameraObserver>();
        Debug.Log(cam2GameObject.name + "에 CameraObserver 스크립트 부착 완료.");
        // User 카메라에 컴포넌트 추가
        cam3GameObject.AddComponent<CameraUser>();
        Debug.Log(cam3GameObject.name + "에 CameraUser 스크립트 부착 완료.");

        // Utility 큐브에 컴포넌트 추가
        myCube.AddComponent<LoginManager>();
        Debug.Log(myCube.name + "에 LoginManager 스크립트 부착 완료.");
        myCube.AddComponent<DataFetcher>();
        Debug.Log(myCube.name + "에 DataFetcher 스크립트 부착 완료.");
        myCube.AddComponent<SensorFetcher>();
        Debug.Log(myCube.name + "에 SensorFetcher 스크립트 부착 완료.");
        myCube.AddComponent<SensorLogFetcher>();
        Debug.Log(myCube.name + "에 SensorLogFetcher 스크립트 부착 완료.");

        // myCube.AddComponent<Receiver>();
        // Debug.Log(myCube.name + "에 Receiver 스크립트 부착 완료.");
        // myCube.AddComponent<ReceiverJson>();
        // Debug.Log(myCube.name + "에 ReceiverJson 스크립트 부착 완료.");

        // myCube.AddComponent<Communication>();
        // Debug.Log(myCube.name + "에 Communication 스크립트 부착 완료.");
        myCube.AddComponent<UserManager>();
        Debug.Log(myCube.name + "에 UserManager 스크립트 부착 완료.");
        
        myCube.AddComponent<CameraSwitch>();
        Debug.Log(myCube.name + "에 CameraSwitch 스크립트 부착 완료.");
        myCube.AddComponent<CanvasToolBar>();
        Debug.Log(myCube.name + "에 CanvasToolBar 스크립트 부착 완료.");
        myCube.AddComponent<MouseCursor>();
        Debug.Log(myCube.name + "에 MouseCursor 스크립트 부착 완료.");
        myCube.AddComponent<CanvasSideBar>();
        Debug.Log(myCube.name + "에 CanvasSideBar 스크립트 부착 완료.");
        myCube.AddComponent<CanvasDebug>();
        Debug.Log(myCube.name + "에 CanvasDebug 스크립트 부착 완료.");

        UserClick userClickScript = myCube.AddComponent<UserClick>();
        string targetLayerName = "UserClickEvent";
        userClickScript.targetLayer = LayerMask.GetMask(targetLayerName);
        Debug.Log(myCube.name + "에 UserClick 스크립트 부착 완료.");

        // myCube.AddComponent<UserReplay>();
        // Debug.Log(myCube.name + "에 UserReplay 스크립트 부착 완료.");

        // GameObject에 컴포넌트 추가 김명권
        GameObject targetObject = GameObject.Find("GameObject");
        UserPositionUpdate newScript = targetObject.AddComponent<UserPositionUpdate>();
        SetPositionNeb spn = targetObject.AddComponent<SetPositionNeb>();
        // 2. 프리팹 불러오기 (Resources 폴더에 있어야 함)
        GameObject prefab = Resources.Load<GameObject>("ControlSystemObject/Player");
        if (prefab == null)
        {
            Debug.LogError("❌ 프리팹 로딩 실패! Resources/gaeunObject/Player 경로 확인");
        }
        else
        {
            spn.positionPrefab = prefab;
            Debug.Log("✅ 프리팹이 SetPositionNeb에 성공적으로 할당되었습니다.");
        }
        // 3. 프리팹 할당
        spn.positionPrefab = prefab;
    }























    // ✨ 특정 Transform의 자식들 중에서 이름이 특정 문자열로 시작하는 첫 번째 자식을 찾는 헬퍼 함수 (이건 그대로!) ✨
    Transform FindChildStartsWith(Transform parent, string prefix)
    {
        if (parent == null)
        {
            Debug.LogError("FindChildStartsWith 함수에 유효하지 않은 parent Transform이 전달되었습니다.");
            return null;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name.StartsWith(prefix))
            {
                Debug.Log("'" + parent.name + "'의 자식 중에서 '" + prefix + "'로 시작하는 '" + child.name + "' 찾음.");
                return child;
            }
        }

        Debug.LogWarning("'" + parent.name + "'의 자식 중에서 이름이 '" + prefix + "'로 시작하는 오브젝트를 찾지 못했습니다.");
        return null;
    }

    // ✨ 특정 Transform의 자식들 중에서 이름이 정확히 일치하는 첫 번째 자식을 찾는 새로운 헬퍼 함수 추가 ✨
    Transform FindChildByName(Transform parent, string targetName)
    {
        if (parent == null)
        {
            Debug.LogError("FindChildByName 함수에 유효하지 않은 parent Transform이 전달되었습니다.");
            return null;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == targetName) // 이름이 정확히 일치하는지 확인!
            {
                Debug.Log("'" + parent.name + "'의 자식 중에서 이름이 '" + targetName + "'인 '" + child.name + "' 찾음.");
                return child;
            }
        }

        Debug.LogWarning("'" + parent.name + "'의 자식 중에서 이름이 '" + targetName + "'인 오브젝트를 찾지 못했습니다.");
        return null;
    }

    // ✨ 수정! ModelingName -> 각 층 -> "Floors" -> "Floor_" 구조를 반영한 위치 찾기 함수 ✨
    void FindAndPrintFloorPositionsWithFloorsChild()
    {
        Debug.Log("ModelingName 오브젝트 및 'Floors' 자식을 거쳐 'Floor_' 오브젝트 위치 찾기 시작! 🔍");

        // 1. "ModelingName" 오브젝트 찾기 (이 부분은 동일)
        GameObject SceneObject = GameObject.Find(ModelingName);
        if (SceneObject == null)
        {
            Debug.LogError($"씬에서 {ModelingName} 이름을 가진 게임 오브젝트를 찾을 수 없어요! 이름이 정확한지 확인해주세요! 😫");
            return;
        }
        Transform SceneTransform = SceneObject.transform;

        // 2. "ModelingName" 오브젝트의 자식들 확인 (이 부분도 동일)
        if (SceneTransform.childCount < 2)
        {
            Debug.LogError($"'{ModelingName}' 오브젝트에 자식 오브젝트가 충분하지 않아요! Hierarchy에 최소 2개 층 오브젝트가 있어야 합니다. 🤔");
            return;
        }

        // 3. Hierarchy 상 가장 위에 있는 자식 = 가장 낮은 층 건물 오브젝트 추정
        Transform lowestBuildingFloorObjectTransform = SceneTransform.GetChild(0);

        // 4. Hierarchy 상 가장 아래에 있는 자식 = 가장 높은 층 건물 오브젝트 추정
        Transform highestBuildingFloorObjectTransform = SceneTransform.GetChild(SceneTransform.childCount - 1);

        Debug.Log("Hierarchy 상단 자식 (가장 낮은 층 건물 추정): " + lowestBuildingFloorObjectTransform.name);
        Debug.Log("Hierarchy 하단 자식 (가장 높은 층 건물 추정): " + highestBuildingFloorObjectTransform.name);
        maxFloor = ParseFloorNumber(highestBuildingFloorObjectTransform.name);
        minFloor = ParseFloorNumber(lowestBuildingFloorObjectTransform.name);

        // ✨ 5. '가장 낮은 층 건물 오브젝트'의 자식들 중에서 이름이 "Floors"인 오브젝트 찾기! ✨
        Transform lowestFloorContainerTransform = FindChildByName(lowestBuildingFloorObjectTransform, "Floors");

        // ✨ 6. '가장 높은 층 건물 오브젝트'의 자식들 중에서 이름이 "Floors"인 오브젝트 찾기! ✨
        Transform highestFloorContainerTransform = FindChildByName(highestBuildingFloorObjectTransform, "Floors");


        // 7. 찾은 "Floors" 오브젝트들 안에서 "Floor_"로 시작하는 오브젝트 다시 찾기!
        lowestFloorPosition = Vector3.zero;
        highestFloorPosition = Vector3.zero;

        // 가장 낮은 층의 "Floors" 컨테이너를 찾았으면 그 안에서 "Floor_" 찾기
        if (lowestFloorContainerTransform != null)
        {
            Debug.Log("가장 낮은 층 건물 ('" + lowestBuildingFloorObjectTransform.name + "') 안에서 'Floors' 오브젝트 찾음.");
            // ✨ FindChildStartsWith 함수 재활용! 'Floors' 오브젝트를 부모로 넘겨줌! ✨
            Transform lowestFloorPositionObjectTransform = FindChildStartsWith(lowestFloorContainerTransform, "Floor_");
            Transform lowestinnerFloorObject = FindChildEndsWith(lowestFloorPositionObjectTransform, "_Floor"); // 신공학관 전용
            if (lowestinnerFloorObject != null)
            {
                lowestFloorPosition = lowestinnerFloorObject.position;
                Debug.Log("✅ 가장 낮은 층 ('" + lowestBuildingFloorObjectTransform.name + "') 내 'Floors' 안의 'Floor_' 오브젝트 위치: " + lowestFloorPosition);
                // 여기 lowestFloorPosition 사용해서 카메라 위치 설정!
            }
            else
            {
                Debug.LogError("가장 낮은 층 건물 ('" + lowestBuildingFloorObjectTransform.name + "') 안의 'Floors' 오브젝트 내에서 이름이 'Floor_'로 시작하는 자식 오브젝트를 찾을 수 없어요! 😫 위치를 알 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("가장 낮은 층 건물 ('" + lowestBuildingFloorObjectTransform.name + "') 오브젝트 내에서 이름이 'Floors'인 자식 오브젝트를 찾을 수 없어요! 😫 'Floor_' 오브젝트를 찾을 수 없습니다.");
        }


        // 가장 높은 층의 "Floors" 컨테이너를 찾았으면 그 안에서 "Floor_" 찾기
        if (highestFloorContainerTransform != null)
        {
            Debug.Log("가장 높은 층 건물 ('" + highestBuildingFloorObjectTransform.name + "') 안에서 'Floors' 오브젝트 찾음.");
            // ✨ FindChildStartsWith 함수 재활용! 'Floors' 오브젝트를 부모로 넘겨줌! ✨
            Transform highestFloorPositionObjectTransform = FindChildStartsWith(highestFloorContainerTransform, "Floor_");
            Transform highestinnerFloorObject = FindChildEndsWith(highestFloorPositionObjectTransform, "_Floor"); // 신공학관 전용
            if (highestFloorPositionObjectTransform != null)
            {
                // highestFloorPosition = highestFloorPositionObjectTransform.position;
                highestFloorPosition = highestinnerFloorObject.position;
                Debug.Log("✅ 가장 높은 층 ('" + highestBuildingFloorObjectTransform.name + "') 내 'Floors' 안의 'Floor_' 오브젝트 위치: " + highestFloorPosition);
                // 여기 highestFloorPosition 사용해서 카메라 위치 설정!
            }
            else
            {
                Debug.LogError("가장 높은 층 건물 ('" + highestBuildingFloorObjectTransform.name + "') 안의 'Floors' 오브젝트 내에서 이름이 'Floor_'로 시작하는 자식 오브젝트를 찾을 수 없어요! 😫 위치를 알 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("가장 높은 층 건물 ('" + highestBuildingFloorObjectTransform.name + "') 오브젝트 내에서 이름이 'Floors'인 자식 오브젝트를 찾을 수 없어요! 😫 'Floor_' 오브젝트를 찾을 수 없습니다.");
        }


        Debug.Log("'Floor_' 오브젝트 위치 찾기 완료!");
        Debug.Log($"높은 층 좌표: {highestFloorPosition}");
        Debug.Log($"낮은 층 좌표: {lowestFloorPosition}");
        // 만약 위치를 찾았다면, 여기서 카메라 위치를 실제로 설정해주는 코드를 추가하면 돼!
        if (overviewCameraTransform != null && highestFloorPosition != null)
        {
            overviewCameraTransform.position = highestFloorPosition + new Vector3(0, 315, 0); // 예시 위치 조정
            Debug.Log("Overview 카메라 위치 설정 완료: " + overviewCameraTransform.position);
        }
        if (observerCameraTransform != null && highestFloorPosition != null)
        {
            observerCameraTransform.position = highestFloorPosition + new Vector3(0, 100, -195); // 예시 위치 조정
            Debug.Log("observer 카메라 위치 설정 완료: " + observerCameraTransform.position);
        }
        if (userCameraTransform != null && highestFloorPosition != null && lowestFloorPosition != null)
        {
            userCameraTransform.position = highestFloorPosition - new Vector3(0, 125, 0);;
            Debug.Log("User 카메라 위치 설정 완료: " + userCameraTransform.position);
        }
    }

    // 층 문자열 투 정수
    int ParseFloorNumber(string floorObjectName)
    {
        if (string.IsNullOrEmpty(floorObjectName))
        {
            Debug.LogWarning("ParseFloorNumber 함수에 빈 문자열이 전달되었습니다.");
            return 0; // 빈 문자열이면 0 또는 다른 기본값 반환
        }

        string numberString = floorObjectName; // 임시 변수에 복사해서 작업

        // 1. "B"로 시작하면 "B"를 "-"로 대체
        if (numberString.StartsWith("B"))
        {
            numberString = "-" + numberString.Substring(1); // "B"를 "-"로 바꾸고 나머지 문자열 붙이기
            Debug.Log("이름 '" + floorObjectName + "'에서 'B'를 '-'로 대체 -> '" + numberString + "'");
        }

        // 2. 가장 마지막 문자(F) 버리기
        if (numberString.EndsWith("F") && numberString.Length > 1) // 'F'로 끝나고 길이가 1보다 길 때만 (예: 그냥 "F" 같은 이름 방지)
        {
            numberString = numberString.Substring(0, numberString.Length - 1); // 마지막 문자 제거
             Debug.Log("이름 '" + floorObjectName + "'에서 마지막 'F' 제거 -> '" + numberString + "'");
        }
        else if (numberString.EndsWith("F") && numberString.Length == 1)
        {
             Debug.LogWarning("이름 '" + floorObjectName + "'은 'F' 한 글자입니다. 파싱할 숫자가 없습니다. 0 반환.");
             return 0; // 'F' 한 글자면 파싱할 숫자가 없으니 0 반환
        }


        int floorNumber = 0; // 변환 결과를 저장할 변수, 기본값 0
        // 3. 남은 문자열을 int로 파싱 (TryParse 사용!)
        if (int.TryParse(numberString, out floorNumber))
        {
            Debug.Log("'" + floorObjectName + "' 이름에서 최종 파싱된 숫자: " + floorNumber);
            return floorNumber; // 파싱 성공! 변환된 숫자 반환
        }
        else
        {
            Debug.LogError("'" + floorObjectName + "' 이름에서 추출된 문자열 '" + numberString + "'을(를) int로 변환하는데 실패했어요! 0 반환.");
            return 0; // 파싱 실패! 0 또는 다른 에러 코드 반환
        }
    }

    // 신공학관 전용
    private Transform FindChildEndsWith(Transform parent, string suffix)
    {
        foreach (Transform child in parent)
        {
            if (child.name.EndsWith(suffix))
            {
                return child;
            }
        }
        return null;
    }
}
