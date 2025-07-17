using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class UserPanelUtil
{
    public static GameObject SetupUserPanel(
        GameObject canvas,
        out GameObject onlineUserContent,
        out GameObject offlineUserContent,
        out TextMeshProUGUI onlineTitleText,
        out TextMeshProUGUI offlineTitleText)
    {
     
        GameObject userPanel = CanvasComponent.CreatePanel(
            canvas,
            "UserListPanel",
            new Vector2(0.12f, 0.92f),          // anchorMin
            new Vector2(0.12f, 0.92f),            // anchorMax
            new Vector2(0f, 1f),            // pivot
            new Color(0f, 0f, 0f, 0.7f),    // background
            new Vector2(0.15f, 0.6f),       // sizePercent (가로 30%, 세로 50%)
            false
        );
        // 뒤로가기 버튼 생성
        float backBtn_xPosPercent = 0.01f;
        float backBtn_yPosPercent = 0.01f;
        float backBtn_widthPercent = 0.3f;
        float backBtn_heightPercent = 0.05f;
        GameObject backBtn = CanvasComponent.CreateButton(
            userPanel,
            "BackButton",
            "닫기",
            backBtn_xPosPercent,
            backBtn_yPosPercent,
            backBtn_widthPercent,
            backBtn_heightPercent
        );

        backBtn.GetComponent<Button>().onClick.AddListener(() => {
            userPanel.SetActive(false);
        });

        // 온라인 사용자
        GameObject onlineTitle = CanvasComponent.AddText(userPanel, $"온라인 사용자 (0)");
        onlineTitleText = onlineTitle.GetComponent<TextMeshProUGUI>();
        onlineTitle.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70);

        GameObject onlineScroll = CreateUserScrollView(userPanel, new Vector2(10, -100));
        onlineUserContent = CreateUserScrollContent(onlineScroll);

        // 오프라인 사용자
        GameObject offlineTitle = CanvasComponent.AddText(userPanel, $"오프라인 사용자 (0)");
        offlineTitleText = offlineTitle.GetComponent<TextMeshProUGUI>();
        offlineTitle.GetComponent<RectTransform>().anchoredPosition = new Vector2(10, -320);

        GameObject offlineScroll = CreateUserScrollView(userPanel, new Vector2(10, -360));
        offlineUserContent = CreateUserScrollContent(offlineScroll);

        return userPanel;
    }

    private static GameObject CreateUserScrollView(GameObject parent, Vector2 anchoredPos)
    {
        GameObject scroll = new GameObject("ScrollView");
        scroll.transform.SetParent(parent.transform, false);
        RectTransform rect = scroll.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = new Vector2(-20, 200);
        scroll.AddComponent<Image>().color = new Color(0, 0, 0, 0);
        scroll.AddComponent<RectMask2D>();
        ScrollRect scrollRect = scroll.AddComponent<ScrollRect>();
        scrollRect.vertical = true;
        scrollRect.horizontal = false;
        scrollRect.scrollSensitivity = 10f;
        return scroll;
    }

    private static GameObject CreateUserScrollContent(GameObject scrollObj)
    {
        GameObject content = new GameObject("ScrollContent");
        content.transform.SetParent(scrollObj.transform, false);
        RectTransform rect = content.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(0, 0);

        var layout = content.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.spacing = 12;
        layout.padding = new RectOffset(10, 10, 10, 10);

        content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scrollObj.GetComponent<ScrollRect>().content = rect;

        return content;
    }
}
