using UnityEngine;

public class MobileCameraController : MonoBehaviour
{
    [Header("대상 설정")]
    public Transform target;            // "Player" 객체의 Transform
    public Vector3 offset = new Vector3(0, 3, -10); // 카메라 초기 오프셋

    [Header("회전 설정")]
    public float verticalRotationSpeed = 0.1f; // 세로 회전 속도
    public float smoothSpeed = 10f;     // 카메라 움직임 부드러움

    [Header("줌 설정")]
    public float minZoomDistance = 0.1f;
    public float maxZoomDistance = 100000f;
    public float baseZoomSpeed = 0.5f;
    public float nearZoomMultiplier = 0.3f;
    public float farZoomMultiplier = 2.0f;

    [Header("자이로 필터 설정")]
    public float alpha = 0.7f;          // LPF 필터 계수 (0~1, 낮을수록 부드러움)

    // 내부 변수
    private float currentX = 0f;
    private float currentY = 30f; // 초기 pitch 값을 40도로 설정
    private float currentDistance;
    private Vector3 desiredPosition;
    private Quaternion desiredRotation;

    // 부드러운 LookAt 타겟을 위한 변수
    private Vector3 smoothedTargetPosition;

    // 터치 관련 변수
    private Touch touchZero;
    private Touch touchOne;
    private Vector2 touchZeroPrevPos;
    private Vector2 touchOnePrevPos;
    private float prevTouchDeltaMag;
    private float touchDeltaMag;

    // 디버그용 변수
    private float lastReceivedGyroValue = -1f;

    // LPF 필터 변수
    private float filteredGyroAngle = -1f;
    private bool isFirstGyroValue = true;

    // Main Camera가 GameObject를 따라갈지 여부
    private bool shouldFollowTarget = false;
    // SetPositionNeb 스크립트 참조
    private SetPositionNeb positionManager;

    void Start()
    {
        if (target == null)
        {
            GameObject playerObject = GameObject.Find("GameObject")?.transform.Find("Player")?.gameObject;
            if (playerObject != null)
            {
                target = playerObject.transform;
                //Debug.Log("플레이어 객체를 자동으로 찾았습니다: " + target.name);
            }
            else
            {
                //Debug.LogError("플레이어 객체를 찾을 수 없습니다. 직접 할당해주세요.");
            }
        }

        currentDistance = 200;
        currentX = 0;
        currentY = 40;

        // 초기 LookAt 타겟을 target의 현재 위치로 설정 (높이 보정 포함)
        if (target != null)
            smoothedTargetPosition = target.position + Vector3.up * (offset.y / 2);

        // 디버그 로그
        //Debug.Log("MobileCameraController가 초기화되었습니다. 게임 오브젝트 이름: " + gameObject.name);

        // SetPositionNeb 스크립트 참조 가져오기
        positionManager = GameObject.Find("GameObject").GetComponent<SetPositionNeb>();
        if (positionManager != null)
        {
            //Debug.Log("SetPositionNeb 스크립트를 찾았습니다.");
        }
        else
        {
            //Debug.LogWarning("SetPositionNeb 스크립트를 찾을 수 없습니다.");
        }
    }

    void LateUpdate()
    {
        // 위치가 업데이트되었는지 확인 (ShowMyPosition이 호출되었는지)
        if (positionManager != null && positionManager.HasPositionBeenUpdated())
        {
            shouldFollowTarget = true;
        }
        if (positionManager != null)
        {
            float camDist = positionManager.cameraDistance;

            if (camDist >= 0f && camDist < 3f)
            {
                currentDistance = 200f;
            }
            else if (camDist >= 3f && camDist < 6.5f)
            {
                currentDistance = 300f;
            }
            else if (camDist >= 6.5f && camDist < 10f)
            {
                currentDistance = 400f;
            }
            else if (camDist >= 10f)
            {
                currentDistance = 500f;
            }

            // res_distance 색상 반영
            positionManager.UpdatePlayerColorByDistance();
        }

        // 대상 추적이 활성화된 경우에만 카메라 업데이트
        if (shouldFollowTarget && target != null)
        {
            // 기존 카메라 업데이트 코드...
            HandleTouchInput();
            desiredRotation = Quaternion.Euler(currentY, currentX, 0);

            Vector3 direction = new Vector3(0, 0, -currentDistance);
            desiredPosition = target.position + (desiredRotation * direction) + new Vector3(0, offset.y, 0);

            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            Vector3 targetLookAt = target.position + Vector3.up * (offset.y / 2);
            smoothedTargetPosition = Vector3.Lerp(smoothedTargetPosition, targetLookAt, smoothSpeed * Time.deltaTime);

            transform.LookAt(smoothedTargetPosition);
        }
    }

    // 안드로이드에서 호출할 공개 함수 - LPF 필터 적용
    public void SetCameraYawRotation(string gyroAngleString)
    {
        // 디버그 로그
        //Debug.Log("SetCameraYawRotation 호출됨: " + gyroAngleString);

        float newGyroAngle;
        if (float.TryParse(gyroAngleString, out newGyroAngle))
        {
            newGyroAngle = newGyroAngle + 90f;
            // 파싱 성공
            // LPF(Low Pass Filter) 적용
            if (isFirstGyroValue)
            {
                // 최초 값은 그대로 적용
                filteredGyroAngle = newGyroAngle;
                isFirstGyroValue = false;
            }
            else
            {
                // LPF 공식: filteredValue = filteredValue + alpha * (newValue - filteredValue)
                float deltaAngle = Mathf.DeltaAngle(filteredGyroAngle, newGyroAngle);
                filteredGyroAngle = filteredGyroAngle + alpha * deltaAngle;
            }

            // 필터링된 값을 카메라 회전에 적용
            currentX = filteredGyroAngle;
            lastReceivedGyroValue = newGyroAngle; // 원시 값 저장 (디버그용)

            //Debug.Log("원시 자이로 각도: " + newGyroAngle + ", 필터링된 각도: " + filteredGyroAngle);
        }
        else
        {
            // 파싱 실패
            //Debug.LogError("자이로 각도 파싱 실패: " + gyroAngleString);
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            // 한 손가락 터치 (수직 회전만)
            touchZero = Input.GetTouch(0);
            if (touchZero.phase == TouchPhase.Moved)
            {
                // 수직 회전만 적용
                currentY -= touchZero.deltaPosition.y * verticalRotationSpeed;
                currentY = Mathf.Clamp(currentY, -80f, 80f);

                // 좌우 이동은 무시 (자이로스코프가 담당)
            }
        }
        else if (Input.touchCount == 2)
        {
            // 두 손가락 터치 (줌)
            touchZero = Input.GetTouch(0);
            touchOne = Input.GetTouch(1);

            if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
            {
                touchZeroPrevPos = touchZero.position;
                touchOnePrevPos = touchOne.position;
                prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            }
            else if (touchZero.phase == TouchPhase.Moved || touchOne.phase == TouchPhase.Moved)
            {
                Vector2 touchZeroCurrentPos = touchZero.position;
                Vector2 touchOneCurrentPos = touchOne.position;
                touchDeltaMag = (touchZeroCurrentPos - touchOneCurrentPos).magnitude;
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                float distanceRatio = Mathf.InverseLerp(minZoomDistance, maxZoomDistance, currentDistance);
                float dynamicZoomSpeed = Mathf.Lerp(baseZoomSpeed * nearZoomMultiplier, baseZoomSpeed * farZoomMultiplier, distanceRatio);

                currentDistance += deltaMagnitudeDiff * dynamicZoomSpeed * 0.01f;
                currentDistance = Mathf.Clamp(currentDistance, minZoomDistance, maxZoomDistance);

                touchZeroPrevPos = touchZeroCurrentPos;
                touchOnePrevPos = touchOneCurrentPos;
                prevTouchDeltaMag = touchDeltaMag;
            }
        }
    }

    // 디버그용 함수: 현재 사용 중인 자이로 값 화면에 표시
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), "원시 자이로 값: " + lastReceivedGyroValue);
        GUI.Label(new Rect(10, 30, 300, 20), "필터링된 자이로 값: " + filteredGyroAngle);
        GUI.Label(new Rect(10, 50, 300, 20), "현재 카메라 Y 회전: " + currentY);
        GUI.Label(new Rect(10, 70, 300, 20), "LPF 알파 값: " + alpha);
    }
}