using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasToolBar : MonoBehaviour
{
    public TextMeshProUGUI viewpointText; // 사용자 카메라의 시점 출력
    public TextMeshProUGUI ToolBaruserIdText;
    // 참조
    public CanvasSideBar canvassidebar;

    void Start()
    {
        canvassidebar = FindObjectOfType<CanvasSideBar>();

        Debug.Log("툴바 Canvas 생성 시작");

        GameObject canvas = CanvasComponent.CreateNewCanvas("Canvas_ToolBar");

        // 전체 화면 상단 8%에 배경 바 생성
        CanvasComponent.CreateTopBarBackground(canvas, new Color(0.1f, 0.18f, 0.4f));

        // 좌측 텍스트
        CanvasComponent.CreateTextOnCanvas(canvas, "Fifth Dimension & Korea Univ.", Color.white, "Institution", new Vector2(0.0f, 1.0f));

        // 중앙 텍스트 (뷰포인트)
        CanvasComponent.CreateTextOnCanvas(canvas, "", Color.white, "ViewPoint", new Vector2(0.5f, 1.0f));

        // 뷰포인트 참조 저장
        GameObject UserViewPointTextBox = GameObject.Find("ViewPoint");
        viewpointText = UserViewPointTextBox.GetComponent<TextMeshProUGUI>();
        viewpointText.gameObject.SetActive(true);

        // 사용자 ID 문자열 준비
        string userIdText = string.IsNullOrEmpty(canvassidebar.ClickedUserID) ? "" : canvassidebar.ClickedUserID;

        // 텍스트 생성 후, TextMeshProUGUI 컴포넌트 참조 저장
        GameObject userInfoGO = CanvasComponent.CreateTextOnCanvas(canvas, userIdText, Color.white, "ToolBarUserInfo", new Vector2(0.3f, 1.0f));
        ToolBaruserIdText = userInfoGO.GetComponent<TextMeshProUGUI>();
    }
}
