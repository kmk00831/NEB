//시작 시 오브젝트 비활성화
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObjects : MonoBehaviour
{
    public List<GameObject> objectsToHideAtStart; // 인스펙터에서 숨길 오브젝트 지정

    [Header("플레이어 플랫폼 감지 및 불투명도 조절")]
    public GameObject player; // 플레이어 오브젝트
    public List<GameObject> platformObjects; // 플레이어가 올라갈 플랫폼들
    public List<GameObject> opacityTargets; // 불투명도 조절 대상 오브젝트들

    // Start is called before the first frame update
    void Start()
    {
        // HideAllObjectsOnStart();
    }

    private void HideAllObjectsOnStart()
    {
        foreach (GameObject obj in objectsToHideAtStart)
        {
            if (obj != null)
            {
                obj.SetActive(false); // 오브젝트 비활성화
            }
        }
    }
}
