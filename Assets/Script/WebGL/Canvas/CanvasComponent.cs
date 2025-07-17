using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public static class CanvasComponent
{
    public static bool LeftSideBarRealTimeUserisActive = false;

    // 새로운 캔버스 생성
public static GameObject CreateNewCanvas(string canvasName)
{
    GameObject canvasGO = new GameObject(canvasName);

    // Canvas 추가 및 설정
    Canvas canvas = canvasGO.AddComponent<Canvas>();
    canvas.renderMode = RenderMode.ScreenSpaceOverlay;

    // CanvasScaler 추가 및 정확한 설정
    CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
    scaler.referenceResolution = new Vector2(1920, 1080); // 기준 해상도
    scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
    scaler.matchWidthOrHeight = 0.5f; // 너비와 높이 비율 균형 맞춤

    // 이벤트 처리용 Raycaster
    canvasGO.AddComponent<GraphicRaycaster>();

    return canvasGO;
}
    // 일단 toolbar의 백그라운드 생성
    public static void CreateTopBarBackground(GameObject canvas, Color bgColor)
    {
        GameObject bg = new GameObject("TopBarBackground");
        bg.transform.SetParent(canvas.transform, false);

        RectTransform rect = bg.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1 - 0.05f); // 화면 상단에서 5% 높이
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = bg.AddComponent<Image>();
        image.color = bgColor;
    }
    // 텍스트 박스 생성
    public static GameObject CreateTextOnCanvas(GameObject canvas, string text, Color textColor, string name, Vector2 anchorPosition)
    {
        GameObject textGO = new GameObject(name);
        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.color = textColor;

        // 폰트 스타일 (한글 출력하려면 필수)
        TMP_FontAsset fontAsset = Resources.Load<TMP_FontAsset>("Font/NanumBarunGothic SDF");
        if (fontAsset != null)
        {
            tmp.font = fontAsset;
        }

        RectTransform rect = textGO.GetComponent<RectTransform>();
        rect.SetParent(canvas.transform, false);

        rect.anchorMin = new Vector2(anchorPosition.x, 1);
        rect.anchorMax = new Vector2(anchorPosition.x, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(10, -10);
        rect.sizeDelta = new Vector2(400, 40);

        tmp.fontSize = 28;
        tmp.alignment = TextAlignmentOptions.Left;

        return textGO; // ✅ 모든 경로가 이 줄로 끝나도록!
    }
    // 웨이퍼 생성 (여러 오브젝트를 묶는 부모 오브젝트라고 생각하면 됨)
    public static GameObject CreateWrapper(GameObject parentCanvas, string wrapperName)
    {
        GameObject wrapper = new GameObject(wrapperName);
        RectTransform rect = wrapper.AddComponent<RectTransform>();
        rect.SetParent(parentCanvas.transform, false);

        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(10, -10);
        rect.sizeDelta = new Vector2(200, 50);

        return wrapper;
    }

    // 버튼 생성
    public static GameObject CreateButton(
        GameObject parent,
        string buttonName,
        string buttonText,
        float xPercent,
        float yPercent,
        float widthPercent,
        float heightPercent
    )
    {
        GameObject buttonGO = new GameObject(buttonName);
        buttonGO.transform.SetParent(parent.transform, false);

        // 버튼 구성
        Button button = buttonGO.AddComponent<Button>();
        Image img = buttonGO.AddComponent<Image>();
        img.color = new Color(0.2f, 0.2f, 0.2f, 1.0f);

        RectTransform rect = buttonGO.GetComponent<RectTransform>();

        // 좌상단 기준으로 퍼센트 anchor 설정
        rect.anchorMin = new Vector2(xPercent, 1 - yPercent - heightPercent);
        rect.anchorMax = new Vector2(xPercent + widthPercent, 1 - yPercent);
        rect.pivot = new Vector2(0, 1);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // 텍스트 추가
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);

        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = buttonText;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 24;

        TMP_FontAsset fontAsset = Resources.Load<TMP_FontAsset>("Font/NanumBarunGothic SDF");
        if (fontAsset != null)
        {
            tmp.font = fontAsset;
        }

        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return buttonGO;
    }
    // 버튼 위치 이동
    public static void SetButtonPosition(
        RectTransform btnRT,
        float xPercent,
        float yPercent,
        float widthPercent,
        float heightPercent,
        Vector2 pivot
    )
    {
        btnRT.anchorMin = new Vector2(xPercent, 1 - yPercent - heightPercent);
        btnRT.anchorMax = new Vector2(xPercent + widthPercent, 1 - yPercent);
        btnRT.pivot = pivot;
        btnRT.offsetMin = Vector2.zero;
        btnRT.offsetMax = Vector2.zero;
    }

    // ✅ 기존 방식 유지용 오버로드 (위치/크기 없이)
    public static GameObject CreateButton(GameObject parent, string buttonName, string buttonText)
    {
        return CreateButton(parent, buttonName, buttonText, 0f, 0f, 1f, 0.05f); // 기본값으로 호출
    }

    // 패널 생성
    public static GameObject CreatePanel(
        GameObject parent,
        string name,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 pivot,
        Color bgColor,
        Vector2 sizePercent,
        bool activeOnStart = false
    )
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent.transform, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        panel.AddComponent<CanvasRenderer>();

        Image bg = panel.AddComponent<Image>();
        bg.color = bgColor;

        // 위치 설정
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;

        // 크기 설정 (% 비율 기반)
        float width = Screen.width * sizePercent.x;
        float height = Screen.height * sizePercent.y;
        rect.sizeDelta = new Vector2(width, height);

        rect.anchoredPosition = Vector2.zero;
        panel.SetActive(activeOnStart);

        return panel;
    }

    // 드롭다운 패널 생성
    public static GameObject CreateDropdownPanel(GameObject parent, string name, RectTransform menuButtonRect)
    {
        GameObject panel = new GameObject(name);
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.SetParent(parent.transform, false);

        // 메뉴 버튼의 너비를 가져옴
        float buttonWidth = menuButtonRect.sizeDelta.x;
        float offsetY = menuButtonRect.anchoredPosition.y;

        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(0, offsetY - menuButtonRect.sizeDelta.y); // 버튼 바로 아래
        rect.sizeDelta = new Vector2(buttonWidth, 100); // 높이는 LayoutGroup이 조절하게 하고, 너비만 맞춤

        // 배경 이미지
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.15f, 0.15f, 0.15f, 0.6f);

        // Layout 구성
        VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 5;
        layout.padding = new RectOffset(5, 5, 5, 5);

        ContentSizeFitter fitter = panel.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        panel.SetActive(false); // 시작 시 숨김

        return panel;
    }
    // 드롭다운 패널 열/닫 이벤트
    public static void AddToggleDropdownEvent(Button menuButton, GameObject dropdownPanel)
    {
        menuButton.onClick.AddListener(() =>
        {
            LeftSideBarRealTimeUserisActive = dropdownPanel.activeSelf;
            dropdownPanel.SetActive(!LeftSideBarRealTimeUserisActive);
        });
    }
    // 패널에 패딩(여백) 주기
    public static void SetPaddingByPercent(
        RectTransform parent,
        RectTransform child,
        float xPercent,
        float yPercent
    )
    {
        float parentWidth = parent.rect.width;
        float parentHeight = parent.rect.height;

        float paddingX = parentWidth * xPercent;
        float paddingY = parentHeight * yPercent;

        child.offsetMin = new Vector2(paddingX, paddingY);     // 좌하
        child.offsetMax = new Vector2(-paddingX, -paddingY);   // 우상
    }
    // 패널에 줄 긋기 수박도 아니고 참.
    public static void CreateHorizontalDivider(GameObject parent, float yPercent)
    {
        GameObject line = new GameObject($"Divider_{yPercent}", typeof(RectTransform), typeof(Image));
        line.transform.SetParent(parent.transform, false);

        RectTransform rt = line.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, yPercent);
        rt.anchorMax = new Vector2(1f, yPercent);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(0f, 1f);  // 1px 높이

        Image img = line.GetComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.5f);  // 연한 회색
    }
    // 각 칸마다 텍스트박스 채워 넣는거.
    public static GameObject CreateSensorTextBox(GameObject parent, float yPercent, string content)
    {
        GameObject go = new GameObject($"SensorText_{yPercent}", typeof(RectTransform));
        go.transform.SetParent(parent.transform, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, yPercent);
        rt.anchorMax = new Vector2(1f, yPercent);
        rt.pivot = new Vector2(0f, 0.5f);  // 왼쪽 중간 기준
        rt.sizeDelta = new Vector2(0f, 20f);  // 높이 20px

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = content;
        tmp.fontSize = 22;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.MidlineLeft;

        // 폰트 지정 (있다면)
        TMP_FontAsset fontAsset = Resources.Load<TMP_FontAsset>("Font/NanumBarunGothic SDF");
        if (fontAsset != null) tmp.font = fontAsset;

        return go;
    }
  
    // 드롭다운 패널에 넣어질 서브 버튼 생성
    public static GameObject CreateSubButton(GameObject parent, string name, string label)
    {
        GameObject buttonGO = new GameObject(name);
        buttonGO.transform.SetParent(parent.transform, false);

        Button button = buttonGO.AddComponent<Button>();
        Image img = buttonGO.AddComponent<Image>();
        img.color = new Color(0.3f, 0.3f, 0.3f, 0.6f);

        RectTransform rect = buttonGO.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(140, 30);

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);

        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();

        // 폰트 스타일 (한글 출력하려면 필수)
        TMP_FontAsset fontAsset = Resources.Load<TMP_FontAsset>("Font/NanumBarunGothic SDF");
        if (fontAsset != null)
        {
            tmp.font = fontAsset;
        }

        tmp.text = label;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 20;
        tmp.fontStyle = FontStyles.Bold;

        
        // 버튼 크기 고정
        LayoutElement layout = buttonGO.AddComponent<LayoutElement>();
        layout.preferredWidth = 140;
        layout.preferredHeight = 30;

        // 텍스트 위치
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return buttonGO;
    }

    public static IEnumerator AnimateDropdown(GameObject panel, bool show)
    {
        RectTransform rect = panel.GetComponent<RectTransform>();
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();

        if (cg == null)
        {
            cg = panel.AddComponent<CanvasGroup>();
        }

        float startScaleY = show ? 0f : 1f;
        float endScaleY = show ? 1f : 0f;

        float startAlpha = show ? 0f : 1f;
        float endAlpha = show ? 1f : 0f;

        float duration = 0.3f;
        float time = 0f;

        if (show)
        {
            panel.SetActive(true); // 시작 전에 켜야 애니메이션 보임
        }

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            float eased = Mathf.SmoothStep(0, 1, t);  // 자연스러운 곡선

            float scaleY = Mathf.Lerp(startScaleY, endScaleY, eased);
            float alpha = Mathf.Lerp(startAlpha, endAlpha, eased);

            rect.localScale = new Vector3(1f, scaleY, 1f);
            cg.alpha = alpha;

            yield return null;
        }

        // 종료 후 마지막 상태 보정
        rect.localScale = new Vector3(1f, endScaleY, 1f);
        cg.alpha = endAlpha;

        if (!show)
        {
            panel.SetActive(false); // 애니메이션 끝나고 나서 비활성화!
        }
    }


    public static void PlayDropdownAnimation(MonoBehaviour runner, GameObject panel, bool show)
    {
        runner.StartCoroutine(AnimateDropdown(panel, show));
    }

    // 텍스트 추가
    public static GameObject AddText(GameObject parent, string content)
    {
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(parent.transform, false);

        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = content;
        tmp.fontSize = 24;
        tmp.color = Color.white;

        // 폰트 스타일 (한글 출력하려면 필수)
        TMP_FontAsset fontAsset = Resources.Load<TMP_FontAsset>("Font/NanumBarunGothic SDF");
        if (fontAsset != null)
        {
            tmp.font = fontAsset;
        }

        var rect = textGO.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = Vector2.zero;

        return textGO; // 🔁 반환 추가
    }

    // 스크롤 생성
    public static GameObject CreateScrollView(
        GameObject parent,
        string name,
        Vector2 anchoredPosition,
        Vector2 padding  // new Vector2(leftRight, topBottom)
    )
    {
        GameObject scroll = new GameObject(name);
        scroll.transform.SetParent(parent.transform, false);

        RectTransform parentRect = parent.GetComponent<RectTransform>();
        RectTransform scrollRect = scroll.AddComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0, 1);
        scrollRect.anchorMax = new Vector2(0, 1);
        scrollRect.pivot = new Vector2(0, 1);
        scrollRect.anchoredPosition = anchoredPosition;

        // 패널 사이즈에서 패딩을 뺀 크기
        Vector2 panelSize = parentRect.rect.size;
        scrollRect.sizeDelta = new Vector2(
            panelSize.x - padding.x,
            panelSize.y - padding.y
        );

        scroll.AddComponent<Image>().color = new Color(0, 0, 0, 0); // 투명
        scroll.AddComponent<RectMask2D>();

        ScrollRect sr = scroll.AddComponent<ScrollRect>();
        sr.vertical = true;
        sr.horizontal = false;
        sr.scrollSensitivity = 10f;

        return scroll;
    }
    // 스크롤 콘텐츠 제거
    public static GameObject CreateScrollContent(
        GameObject scrollParent,
        out RectTransform contentRect,
        out VerticalLayoutGroup layoutGroup
    )
    {
        GameObject content = new GameObject("FloorContent");
        content.transform.SetParent(scrollParent.transform, false);

        contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0, 1);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(0, 0);

        layoutGroup = content.AddComponent<VerticalLayoutGroup>();
        layoutGroup.childAlignment = TextAnchor.UpperLeft;
        layoutGroup.spacing = 10f;
        layoutGroup.padding = new RectOffset(10, 10, 10, 10);

        ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        return content;
    }
}   
