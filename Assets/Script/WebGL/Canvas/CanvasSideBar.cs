using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class CanvasSideBar : MonoBehaviour
{
    private bool menuExpanded = false;
    private GameObject userPanel;
    private GameObject onlineUserContent;
    private GameObject offlineUserContent;
    private GameObject floorPanel;
    private GameObject floorContent;

    private TextMeshProUGUI offlineTitleText;
    private TextMeshProUGUI onlineTitleText;
    
    // 버튼 제어를 위한 전역 처리
    private GameObject userBtn;
    private GameObject floorBtn;
    // 버튼 클릭 시, 강조 처리
    private Button lastSelectedUserButton = null;
    private Color userDefaultColor = new Color(0.2f, 0.2f, 0.2f);
    private Color userSelectedColor = new Color(0.3f, 0.5f, 0.8f);
    //  선택된 사용자 id 저장
    public string ClickedUserID = null;
    // 참조
    public Setup setup;
    public SetPositionNeb setpositionneb;
    public CanvasToolBar canvastoolbar;
    private Dictionary<int, TextMeshProUGUI> floorBadges;
    private Coroutine userListCoroutine;
    public Dictionary<int, int> floorOnlineCounts = new(); // 층별 온라인 유저 컨트
    private Dictionary<int, TextMeshProUGUI> floorLabels;


    void Start()
    {
        setup = FindObjectOfType<Setup>();
        setpositionneb = FindObjectOfType<SetPositionNeb>();
        canvastoolbar = FindObjectOfType<CanvasToolBar>();
        GameObject canvas = CanvasComponent.CreateNewCanvas("Canvas_SideBar");

        // ✅ Menu 버튼 생성
        float Menu_xPosPercent = 0.01f;
        float Menu_yPosPercent = 0.08f;
        float Menu_widthPercent = 0.1f; 
        float Menu_heightPercent = 0.06f;
        GameObject menuBtn = CanvasComponent.CreateButton(
            canvas,
            "MenuButton",
            "Menu ⏶",
            Menu_xPosPercent,    // xPercent: 왼쪽 끝
            Menu_yPosPercent,    // yPercent: 상단에서 8% 아래
            Menu_widthPercent,   // widthPercent: 전체 너비
            Menu_heightPercent   // heightPercent: 6%
        );

        // ✅ 층 이동 버튼 생성
        float Floor_xPosPercent = 0.01f;
        float Floor_yPosPercent = 0.15f;
        float Floor_widthPercent = 0.1f;
        float Floor_heightPercent = 0.06f;
        floorBtn = CanvasComponent.CreateButton(
            canvas,
            "FloorButton",
            "층",
            Floor_xPosPercent,  // xPercent (왼쪽에서 1%)
            Floor_yPosPercent,  // yPercent (상단에서 12% 아래)
            Floor_widthPercent,  // widthPercent (5%)
            Floor_heightPercent   // heightPercent (6%)
        );

        // ✅ 사용자 버튼 생성
        float User_xPosPercent = 0.01f;
        float User_yPosPercent = 0.22f;
        float User_widthPercent = 0.1f;
        float User_heightPercent = 0.06f;
        userBtn = CanvasComponent.CreateButton(
            canvas, 
            "UserButton", 
            "사용자", 
            User_xPosPercent, 
            User_yPosPercent, 
            User_widthPercent, 
            User_heightPercent
        );
        floorBtn.SetActive(false); // 초기 버튼 상태
        userBtn.SetActive(false); // 초기 버튼 상태
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        // 사용자 패널 세팅
        userPanel = UserPanelUtil.SetupUserPanel(
            canvas,
            out onlineUserContent,
            out offlineUserContent,
            out onlineTitleText,
            out offlineTitleText
        );

        // 층 이동 패널 세팅
        floorPanel = FloorPanel.SetupFloorPanel(
            canvas,
            setup,
            setpositionneb,
            userPanel,
            out floorContent,
            out floorBadges,
            out floorLabels        
        );
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        // 버튼 이벤트 연결
        // 메뉴
        menuBtn.GetComponent<Button>().onClick.AddListener(() => {
            menuExpanded = !menuExpanded;

            floorBtn.SetActive(menuExpanded);
            userBtn.SetActive(menuExpanded);

            if (menuExpanded)
            {
                if (userListCoroutine == null)
                    userListCoroutine = StartCoroutine(UpdateUserListRoutine());
            }
            else
            {
                if (userListCoroutine != null)
                {
                    StopCoroutine(userListCoroutine);
                    userListCoroutine = null;
                }
            }
        });
        // 사용자
        userBtn.GetComponent<Button>().onClick.AddListener(() => {
            userPanel.SetActive(true);
            floorPanel.SetActive(false);
        });
        // 층
        floorBtn.GetComponent<Button>().onClick.AddListener(() => {
            floorPanel.SetActive(true);
            userPanel.SetActive(false);
        });

    }















    // 사용자 정보 가져오는거 코루틴으로 만들어 둔 건데 웬만해서 건들지 말 것.
    // 유저 id, 그리고 온/오프라인 상태 명 수를 리스트형으로 가져옴.
    IEnumerator UpdateUserListRoutine()
    {
        while (true)
        {
            if ((userPanel != null && userPanel.activeSelf) || (floorPanel != null && floorPanel.activeSelf))
            {
                PopulateUserList();
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    void PopulateUserList()
    {
        foreach (Transform child in onlineUserContent.transform)
            Destroy(child.gameObject);
        foreach (Transform child in offlineUserContent.transform)
            Destroy(child.gameObject);

        var userManager = UserManager.Instance;
        if (userManager == null) return;


        foreach (var user in userManager.onlineUsers)
        {
            string uid = user.userId;
            GameObject btn = CanvasComponent.CreateButton(onlineUserContent, $"User_{uid}", $"ID: {uid}");
            btn.AddComponent<LayoutElement>().minHeight = 40f;

            Image btnImage = btn.GetComponent<Image>();
            btnImage.color = userDefaultColor;

            Button btnComp = btn.GetComponent<Button>();
            btnComp.onClick.AddListener(() => {
                ClickedUserID = uid.ToString();
                Debug.Log($"▶️ 온라인 사용자 ID: {ClickedUserID}");

                if (lastSelectedUserButton != null)
                    lastSelectedUserButton.GetComponent<Image>().color = userDefaultColor;

                btnImage.color = userSelectedColor;
                lastSelectedUserButton = btnComp;
            });
        }

        foreach (var user in userManager.offlineUsers)
        {
            string uid = user.userId;
            GameObject btn = CanvasComponent.CreateButton(offlineUserContent, $"User_{uid}", $"ID: {uid}");
            btn.AddComponent<LayoutElement>().minHeight = 40f;

            Image btnImage = btn.GetComponent<Image>();
            btnImage.color = userDefaultColor;

            Button btnComp = btn.GetComponent<Button>();
            btn.GetComponent<Button>().onClick.AddListener(() => {
                ClickedUserID = uid.ToString();
                Debug.Log($"▶️ 오프라인 사용자 ID: {ClickedUserID}");

                if (canvastoolbar.ToolBaruserIdText != null)
                    canvastoolbar.ToolBaruserIdText.text = ClickedUserID;

                if (lastSelectedUserButton != null)
                    lastSelectedUserButton.GetComponent<Image>().color = userDefaultColor;

                btnImage.color = userSelectedColor;
                lastSelectedUserButton = btnComp;
            });
        }

        if (offlineTitleText != null)
            offlineTitleText.text = $"오프라인 사용자 ({userManager.offlineUsers.Count})";
        if (onlineTitleText != null)
            onlineTitleText.text = $"온라인 사용자 ({userManager.onlineUsers.Count})";

        foreach (var kv in floorLabels)
        {
            int floor = kv.Key;
            int count = UserManager.Instance.GetOnlineCount(floor);
            floorOnlineCounts[floor] = count;

            kv.Value.text = FormatFloorLabel(floor, count);
        }
    }
    public string FormatFloorLabel(int floor, int count)
    {
        string label = (floor < 0) ? $"B{Mathf.Abs(floor)}층" : $"{floor}층";
        if (count > 0) label += $" ({count})";
        return label;
    }
}
