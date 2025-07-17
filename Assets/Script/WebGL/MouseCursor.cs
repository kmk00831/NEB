using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    public bool isAltPressed = false;

    void Start()
    {
        // 앱 시작 시 마우스 커서 숨기기
        LockCursor();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            isAltPressed = true;
            UnlockCursor();
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt))
        {
            isAltPressed = false;
            LockCursor();
        }
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
