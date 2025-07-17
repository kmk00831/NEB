
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CameraUser : MonoBehaviour
{
    // 카메라 변수
    public Camera User;

    // 추적 타겟
    private Transform target;

    // 카메라 파라미터
    private float height = 25f;
    private float distance = 30f;
    private float lookDownAngle = 15f;
    private float smoothTime = 1.0f;
    private float rotationSmoothTime = 5f;
    private GameObject parentObject;
    private string childObjectName;
    public bool isFirstPersonView = false;
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 previousTargetPosition = Vector3.zero;
    private bool targetFound = false;
    private Quaternion currentRotation;
    private  float one_height = 10f;
    private float one_distance = 2.0f;
    public bool oneTargetFlag = true;
    private float heightScrollSpeed = 5;
    private float minHeight = -60;
    private float maxHeight = 300;
    
    // 객체 참조
    public UserClick userclick;
    public CanvasSideBar canvassidebar;

    // 새로운 변수
    private Vector3 targetMoveDirection = Vector3.forward; // 타겟의 이동 방향
    public float moveDirectionSmoothTime = 0.1f; // 이동 방향 스무딩 시간
    private Vector3 moveDirectionCurrentVelocity = Vector3.zero; // 이동 방향 스무딩 현재 속도

    void Start()
    {
        StartCoroutine(Initialize());
        currentRotation = transform.rotation;
        // 카메라 할당
        GameObject cameraObject = GameObject.Find("Camera(User)");
        User = cameraObject.GetComponent<Camera>();
        // 플레이어 오브젝트 할당
        parentObject = GameObject.Find("GameObject");
    }

    IEnumerator Initialize()
    {
        yield return new WaitUntil(() => FindObjectOfType<UserClick>() != null);
        userclick = FindObjectOfType<UserClick>();
    }

    private void FindTargetObject(string userName)
    {
        if (parentObject != null)
        {
            childObjectName = "USER_" + userName;
            Transform[] childTransforms = parentObject.GetComponentsInChildren<Transform>();
            List<Transform> matchingChildren = childTransforms.Where(child => child.name == childObjectName).ToList();

            if (matchingChildren.Count > 0)
            {
                target = matchingChildren.Last();
                targetFound = true;
            }
            else
            {
                target = null;
            }
        }
        else
        {
            target = null;
        }
    }

    void LateUpdate()
    {
        if (canvassidebar == null)
        {
            return; // userclick이 null이면 LateUpdate 함수를 종료
        }

        if (canvassidebar.ClickedUserID != null)
        {
            FindTargetObject(canvassidebar.ClickedUserID);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            isFirstPersonView = !isFirstPersonView;
        }

        if (target != null)
        {
            Vector3 desiredPosition;
            Quaternion desiredRotation;

            if (isFirstPersonView)
            {
                // // 1인칭 시점
                Vector3 currentMoveDirection = (target.position - previousTargetPosition).normalized;
                if (currentMoveDirection.magnitude > 0) // 이동이 있을 때만 업데이트
                {
                    targetMoveDirection = Vector3.SmoothDamp(targetMoveDirection, currentMoveDirection, ref moveDirectionCurrentVelocity, moveDirectionSmoothTime);
                }

                // 목표 위치 계산 (타겟 이동 방향의 반대쪽으로)
                desiredPosition = target.position - targetMoveDirection * one_distance;
                desiredPosition.y = target.position.y + one_height;
                
                
                Vector3 lookDirection = target.position - transform.position;
                // lookDirection 벡터를 사용해서 일단 원래 회전 값을 구함
                Quaternion tempRotation = Quaternion.LookRotation(lookDirection);
                // 그 회전 값에서 Y랑 Z축 회전 각도만 가져와서 새로운 Quaternion 생성 (X는 0!)
                desiredRotation = Quaternion.Euler(0f, tempRotation.eulerAngles.y, tempRotation.eulerAngles.z); // X축은 0으로 고정! Z축은 원래 값을 사용!

                // 부드러운 회전
                currentRotation = Quaternion.Slerp(currentRotation, desiredRotation, rotationSmoothTime * Time.deltaTime);

                // 위치 업데이트
                transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);
                transform.rotation = currentRotation;

                if (oneTargetFlag) {
                    if(target.GetComponent<SkinnedMeshRenderer>() != null){
                        target.GetComponent<SkinnedMeshRenderer>().enabled = false;
                    }
                    else if(target.GetComponent<MeshRenderer>() != null)
                    {
                        target.GetComponent<MeshRenderer>().enabled = false;
                    }
                }
                else { 
                    if(target.GetComponent<SkinnedMeshRenderer>() != null){
                        target.GetComponent<SkinnedMeshRenderer>().enabled = true;
                    }
                    else if(target.GetComponent<MeshRenderer>() != null)
                    {
                        target.GetComponent<MeshRenderer>().enabled = true;
                    }
                }
            }
            else
            {
                // 3인칭 시점

                // 마우스 휠 입력으로 카메라 높이 조절
                float scrollInput = Input.GetAxis("Mouse ScrollWheel");
                if (Mathf.Abs(scrollInput) > 0.01f)
                {
                    height -= scrollInput * heightScrollSpeed;
                    height = Mathf.Clamp(height, minHeight, maxHeight); // 예: 2f ~ 15f
                }

                // 타겟의 이동 방향 계산 및 스무딩
                Vector3 currentMoveDirection = (target.position - previousTargetPosition).normalized;
                if (currentMoveDirection.magnitude > 0) // 이동이 있을 때만 업데이트
                {
                    targetMoveDirection = Vector3.SmoothDamp(targetMoveDirection, currentMoveDirection, ref moveDirectionCurrentVelocity, moveDirectionSmoothTime);
                }

                // 목표 위치 계산 (타겟 이동 방향의 반대쪽으로)
                desiredPosition = target.position - targetMoveDirection * distance;
                desiredPosition.y = target.position.y + height;

                // 목표 회전 계산 (타겟을 바라보도록)
                desiredRotation = Quaternion.LookRotation(target.position - transform.position);

                // 부드러운 회전
                currentRotation = Quaternion.Slerp(currentRotation, desiredRotation, rotationSmoothTime * Time.deltaTime);

                // 위치 업데이트
                transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);
                transform.rotation = currentRotation;
            }

            previousTargetPosition = target.position;
        }
    }
}
