using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SensorPanel : MonoBehaviour
{
    [SerializeField] RectTransform parentPanel, childPanel;
    [SerializeField] float xPaddingPercent, yPaddingPercent;

    private Dictionary<string, TextMeshProUGUI> texts;
    private float[] yOffsets;
    private Coroutine paddingRoutine, updateRoutine;
    private string currentUserId;
    private SensorFetcher fetcher;
    private DataFetcher locationFetcher;

    public void Init(
        Dictionary<string, TextMeshProUGUI> sensorTextMap,
        float[] offsets,
        RectTransform parent, RectTransform child,
        float xPct, float yPct)
    {
        texts = sensorTextMap;
        yOffsets = offsets;
        parentPanel = parent;
        childPanel = child;
        xPaddingPercent = xPct;
        yPaddingPercent = yPct;

        // 미리 찾아두기 (캐싱)
        fetcher = FindObjectOfType<SensorFetcher>();
        locationFetcher = FindObjectOfType<DataFetcher>();
    }

    public void StartUpdating(string userId)
    {
        currentUserId = userId;
        paddingRoutine = StartCoroutine(ApplyPadding());
        updateRoutine  = StartCoroutine(UpdateLoop());
    }

    public void StopUpdating()
    {
        if (paddingRoutine != null) StopCoroutine(paddingRoutine);
        if (updateRoutine  != null) StopCoroutine(updateRoutine);
    }

    IEnumerator ApplyPadding()
    {
        yield return new WaitForEndOfFrame();
        CanvasComponent.SetPaddingByPercent(parentPanel, childPanel, xPaddingPercent, yPaddingPercent);
    }

    IEnumerator UpdateLoop()
    {
        while (true)
        {
            if (!string.IsNullOrEmpty(currentUserId))
            {
                // 위치 텍스트 갱신
                if (locationFetcher?.latestData != null)
                {
                    var loc = locationFetcher.latestData.FirstOrDefault(u => u.userId == currentUserId);
                    string posStr = loc != null
                        ? $"X: {loc.userX:F2}, Y: {loc.userY:F2}, Z: {loc.userZ:F2}"
                        : "N/A";
                    if (texts.TryGetValue($"SensorText_{yOffsets[19]}", out var t0))
                        t0.text = $"위치: {posStr}";
                }

                // 센서값 텍스트 갱신
                if (fetcher != null && fetcher.sensorDataByUserId.TryGetValue(currentUserId, out var d))
                {
                    if (texts.TryGetValue($"SensorText_{yOffsets[16]}", out var t3))
                        t3.text = $"가속도: ({d.linearAcceleration.x:F2}, {d.linearAcceleration.y:F2}, {d.linearAcceleration.z:F2})";

                    if (texts.TryGetValue($"SensorText_{yOffsets[15]}", out var t4))
                        t4.text = $"자기장: ({d.magnetic.x:F2}, {d.magnetic.y:F2}, {d.magnetic.z:F2})";

                    if (texts.TryGetValue($"SensorText_{yOffsets[14]}", out var t5))
                        t5.text = $"기압: {d.pressure:F2}";

                    if (texts.TryGetValue($"SensorText_{yOffsets[13]}", out var t6))
                        t6.text = $"조도: {d.light:F2}";

                    if (texts.TryGetValue($"SensorText_{yOffsets[12]}", out var t7))
                        t7.text = $"근접: {d.proximity:F2}";

                    if (texts.TryGetValue($"SensorText_{yOffsets[11]}", out var t8))
                        t8.text = $"RF: {d.rf:F2}";
                }
            }

            yield return new WaitForSeconds(0.3f);
        }
    }
}
