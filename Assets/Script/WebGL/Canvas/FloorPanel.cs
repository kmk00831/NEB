using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public static class FloorPanel
{
    public static GameObject SetupFloorPanel(
        GameObject canvas,
        Setup setup,
        SetPositionNeb setpositionneb,
        GameObject userPanel,
        out GameObject floorContent,
        out Dictionary<int, TextMeshProUGUI> floorBadges,
        out Dictionary<int, TextMeshProUGUI> floorLabels)
    {
        // 층 패널 버튼 생성
        GameObject floorPanel = CanvasComponent.CreatePanel(
            canvas,
            "FloorPanel",
            new Vector2(0.12f, 0.92f),          // anchorMin
            new Vector2(0.12f, 0.92f),            // anchorMax
            new Vector2(0f, 1f),           // pivot
            new Color(0f, 0f, 0f, 0.7f),   // 배경색
            new Vector2(0.1f, 0.5f), // sizePercent 계산
            false                          // 초기 비활성화
        );

        // 뒤로가기 버튼 생성
        float backBtn_xPosPercent = 0.01f;
        float backBtn_yPosPercent = 0.01f;
        float backBtn_widthPercent = 0.3f;
        float backBtn_heightPercent = 0.05f;
        GameObject backBtn = CanvasComponent.CreateButton(
            floorPanel,
            "BackButton",
            "닫기",
            backBtn_xPosPercent,
            backBtn_yPosPercent,
            backBtn_widthPercent,
            backBtn_heightPercent
        );
        // 뒤로가기 버튼 클릭
        backBtn.GetComponent<Button>().onClick.AddListener(() => {
            floorPanel.SetActive(false);
            if (userPanel != null) userPanel.SetActive(false);
        });

        GameObject scroll = CanvasComponent.CreateScrollView(
            floorPanel,
            "FloorScrollView",
            new Vector2(10, -60),   // anchoredPosition
            new Vector2(20, 80)     // padding (좌우 10, 상하 60/20)
        );
        
        ScrollRect sr = scroll.GetComponent<ScrollRect>();
        RectTransform scrollRect = scroll.GetComponent<RectTransform>();

        // Content 영역 생성 함수화
        floorContent = CanvasComponent.CreateScrollContent(
            scroll,
            out RectTransform contentRect,
            out VerticalLayoutGroup layout
        );

        sr.content = contentRect;
        sr.viewport = scrollRect;

        // 층 버튼 생성 및 배지 초기화
        floorBadges = new Dictionary<int, TextMeshProUGUI>();
        floorLabels = new Dictionary<int, TextMeshProUGUI>(); // ✅ 초기화

        var sidebar = GameObject.FindObjectOfType<CanvasSideBar>();
        Color defaultColor = new Color(0.2f, 0.2f, 0.2f);
        Color selectedColor = new Color(0.3f, 0.5f, 0.8f);
        Button lastSelectedButton = null;
        for (int i = setup.maxFloor; i >= setup.minFloor; i--)
        {
            if (i == 0) continue;
            int floorValue = i;

            GameObject btn = CanvasComponent.CreateButton(floorContent, $"Floor_{floorValue}", "");
            TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();

            int count = 0;
            if (sidebar != null && sidebar.floorOnlineCounts.TryGetValue(floorValue, out count))
                btnText.text = sidebar.FormatFloorLabel(floorValue, count);
            else
                btnText.text = (i < 0) ? $"B{Mathf.Abs(i)}층" : $"{i}층";

            floorLabels[floorValue] = btnText;

            Button btnComp = btn.GetComponent<Button>();
            Image btnImage = btn.GetComponent<Image>();
            btnImage.color = defaultColor;

            btnComp.onClick.AddListener(() =>
            {
                setpositionneb.FloorController = floorValue;

                if (lastSelectedButton != null)
                    lastSelectedButton.GetComponent<Image>().color = defaultColor;

                btnImage.color = selectedColor;
                lastSelectedButton = btnComp;
            });

            var hlg = btn.AddComponent<HorizontalLayoutGroup>();
            hlg.childAlignment = TextAnchor.UpperLeft;
            RectTransform btnRect = btn.GetComponent<RectTransform>();
            hlg.spacing = 4f; // 안전하게 고정값 사용하거나 해상도 기반 계산
            hlg.padding = new RectOffset(10, 10, 0, 0);

            // 버튼 추가 후 부모의 RectTransform 강제 갱신
            RectTransform parentRect = btn.transform.parent.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);

            float parentWidth = parentRect.rect.width;

            LayoutElement le = btn.AddComponent<LayoutElement>();
            le.preferredHeight = 40f;
            le.preferredWidth = parentWidth * 0.9f;
        }

        return floorPanel;
    }
}
