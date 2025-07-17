using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CanvasDebug : MonoBehaviour
{
    private bool sensorBtnFlag = false;
    private bool terminalBtnFlag = false;

    private GameObject sensorMotherPanel;
    private GameObject sensorDataPanel;

    private Dictionary<string, TextMeshProUGUI> sensorTextByKey = new();
    private readonly float[] yOffsets = new float[20];

    //참조
    public SensorLogFetcher sensorlogfetcher;
    public CanvasSideBar canvassidebar;

    void Start()
    {
        sensorlogfetcher = FindObjectOfType<SensorLogFetcher>();
        canvassidebar = FindObjectOfType<CanvasSideBar>();
        // 캔버스 생성
        GameObject canvas = CanvasComponent.CreateNewCanvas("Canvas_Sensors");

        // 센서 버튼
        GameObject sensorBtn = CanvasComponent.CreateButton(
            parent: canvas,
            buttonName: "SensorPanelBtn",
            buttonText: "<",
            xPercent: 0.985f,
            yPercent: 0.1f,
            widthPercent: 0.015f,
            heightPercent: 0.1f
        );

        // 터미널 버튼
        GameObject terminalBtn = CanvasComponent.CreateButton(
            parent: canvas,
            buttonName: "TerminalPanelBtn",
            buttonText: "<",
            xPercent: 0.985f,
            yPercent: 0.25f,
            widthPercent: 0.015f,
            heightPercent: 0.1f
        );

        // 센서 마더 패널
        sensorMotherPanel = CanvasComponent.CreatePanel(
            canvas,
            "SensorMotherPanel",
            new Vector2(0.7f, 0.95f),
            new Vector2(0.7f, 0.95f),
            new Vector2(0f, 1f),
            new Color(0, 0, 0, 0.7f),
            new Vector2(0.3f, 1.0f),
            false
        );

        // 센서 데이터 패널
        sensorDataPanel = CanvasComponent.CreatePanel(
            sensorMotherPanel,
            "sensorDataPanel",
            new Vector2(0f, 0.03f),
            new Vector2(1f, 0.8f),
            new Vector2(0.0f, 1.0f),
            new Color(0.1f, 0.1f, 0.1f, 0.8f),
            new Vector2(0.3f, 1.0f),
            false
        );

        string[] initialLabels = new string[20];

        // 필요한 위치에 라벨 지정
        initialLabels[19] = "위치: ";
        initialLabels[18] = "핸드폰 상태: ";
        initialLabels[17] = "자이로: ";
        initialLabels[16] = "가속도: ";
        initialLabels[15] = "자기장: ";
        initialLabels[14] = "기압: ";
        initialLabels[13] = "조도: ";
        initialLabels[12] = "근접: ";
        initialLabels[11] = "RF: ";

        // UI 생성 루프
        for (int i = 0; i < 20; i++)
        {
            float y = (i + 1) * 0.05f;
            yOffsets[i] = y - 0.025f;

            CanvasComponent.CreateHorizontalDivider(sensorDataPanel, y);
            
            string defaultText = initialLabels[i] ?? " ";  // 해당 위치에 라벨이 지정돼 있으면, 아니면 빈 칸
            GameObject go = CanvasComponent.CreateSensorTextBox(sensorDataPanel, yOffsets[i], defaultText);

            var text = go.GetComponent<TextMeshProUGUI>();
            sensorTextByKey[$"SensorText_{yOffsets[i]}"] = text;
        }


        // // 버튼 클릭 이벤트
        // sensorBtn.GetComponent<Button>().onClick.AddListener(() =>
        // {
        //     sensorBtnFlag = !sensorBtnFlag;
        //     sensorMotherPanel.SetActive(sensorBtnFlag);
        //     sensorDataPanel.SetActive(sensorBtnFlag);
            
        //     // 센서 로그 코루틴 시작
        //     if(sensorBtnFlag)
        //         if (!string.IsNullOrEmpty(canvassidebar.ClickedUserID))
        //             sensorlogfetcher.StartFetching(canvassidebar.ClickedUserID);
        //     else
        //         sensorlogfetcher.StopFetching();

        //     float x = sensorBtnFlag ? 0.685f : 0.985f;
        //     CanvasComponent.SetButtonPosition(
        //         sensorBtn.GetComponent<RectTransform>(), x, 0.1f, 0.015f, 0.1f, new Vector2(0f, 1f)
        //     );

        //     var tmp = sensorBtn.GetComponentInChildren<TextMeshProUGUI>();
        //     if (tmp != null) tmp.text = sensorBtnFlag ? ">" : "<";
        // });

        terminalBtn.GetComponent<Button>().onClick.AddListener(() =>
        {
            terminalBtnFlag = !terminalBtnFlag;

            float x = terminalBtnFlag ? 0.685f : 0.985f;
            CanvasComponent.SetButtonPosition(
                terminalBtn.GetComponent<RectTransform>(), x, 0.25f, 0.015f, 0.1f, new Vector2(0f, 1f)
            );

            var tmp = terminalBtn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) tmp.text = terminalBtnFlag ? ">" : "<";
        });

        // 패딩 적용
        // StartCoroutine(ApplySensorPanelPadding(sensorMotherPanel, sensorDataPanel, 0.03f, 0.03f));

        // // 실시간 센서 값 업데이트 시작
        // StartCoroutine(UpdateSensorTexts());


        var updater = canvas.AddComponent<SensorPanel>();
        updater.Init(sensorTextByKey, yOffsets,
                    sensorMotherPanel.GetComponent<RectTransform>(),
                    sensorDataPanel.GetComponent<RectTransform>(),
                    0.03f, 0.03f);

        sensorBtn.GetComponent<Button>().onClick.AddListener(() =>
        {
            sensorBtnFlag = !sensorBtnFlag;
            sensorMotherPanel.SetActive(sensorBtnFlag);
            sensorDataPanel.SetActive(sensorBtnFlag);

            if (sensorBtnFlag)
                updater.StartUpdating(canvassidebar.ClickedUserID);
            else
                updater.StopAllCoroutines();
        });
    }

    // IEnumerator ApplySensorPanelPadding(GameObject parentPanel, GameObject childPanel, float xPercent, float yPercent)
    // {
    //     yield return new WaitForEndOfFrame();

    //     RectTransform parentRT = parentPanel.GetComponent<RectTransform>();
    //     RectTransform childRT = childPanel.GetComponent<RectTransform>();

    //     CanvasComponent.SetPaddingByPercent(parentRT, childRT, xPercent, yPercent);
    // }

    // IEnumerator UpdateSensorTexts()
    // {
    //     while (true)
    //     {
    //         string userId = FindObjectOfType<CanvasSideBar>()?.ClickedUserID;

    //         if (!string.IsNullOrEmpty(userId))
    //         {
    //             var fetcher = FindObjectOfType<SensorFetcher>();
    //             var locationFetcher = FindObjectOfType<DataFetcher>();

    //             if (locationFetcher != null && locationFetcher.latestData != null)
    //             {
    //                 var location = locationFetcher.latestData
    //                     .FirstOrDefault(u => u.userId == userId);

    //                 if (location != null)
    //                 {
    //                     string posStr = $"X: {location.userX:F2}, Y: {location.userY:F2}, Z: {location.userZ}";
    //                     UpdateText($"SensorText_{yOffsets[19]}", $"위치: {posStr}");
    //                 }
    //                 else
    //                 {
    //                     UpdateText($"SensorText_{yOffsets[19]}", $"위치: N/A");
    //                 }
    //             }

    //             if (fetcher != null && fetcher.sensorDataByUserId.TryGetValue(userId, out SensorData d))
    //             {
    //                 UpdateText($"SensorText_{yOffsets[18]}", $"핸드폰 상태: {d.userStateReal}");
    //                 UpdateText($"SensorText_{yOffsets[17]}", $"자이로: {Vec3ToStr(d.gyro)}");
    //                 UpdateText($"SensorText_{yOffsets[16]}", $"가속도: {Vec3ToStr(d.linearAcceleration)}");
    //                 UpdateText($"SensorText_{yOffsets[15]}", $"자기장: {Vec3ToStr(d.magnetic)}");
    //                 UpdateText($"SensorText_{yOffsets[14]}", $"기압: {d.pressure}");
    //                 UpdateText($"SensorText_{yOffsets[13]}", $"조도: {d.light}");
    //                 UpdateText($"SensorText_{yOffsets[12]}", $"근접: {d.proximity}");
    //                 UpdateText($"SensorText_{yOffsets[11]}", $"RF: {d.rf}");

    //                 // UpdateText($"SensorText_{yOffsets[4]}", $"rot: {Vec3ToStr(d.rotation)}"); // 회전임
    //                 // 나머지 칸은 원하면 더 채워도 됨
    //             }
    //         }

    //         yield return new WaitForSeconds(0.3f);
    //     }
    // }

    // void UpdateText(string key, string value)
    // {
    //     if (sensorTextByKey.TryGetValue(key, out var text))
    //     {
    //         text.text = value;
    //     }
    // }

    // string Vec3ToStr(Vector3Data v) => $"({v.x:F2}, {v.y:F2}, {v.z:F2})";
}
