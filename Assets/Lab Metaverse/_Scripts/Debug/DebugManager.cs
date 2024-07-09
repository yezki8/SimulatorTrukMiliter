using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

#if UNITY_EDITOR
public class DebugManager : MonoBehaviour
{
    [SerializeField] private TMP_Text debugText;
    [SerializeField] private float updateInterval = 0.5f;

    private StringBuilder stringBuilder = new StringBuilder();

    private float fps;
    private float frameTime;
    private int frameCount;
    private float elapsedTime;

    private void Start()
    {
        StartCoroutine(UpdateDebugInfo());
    }

    private IEnumerator UpdateDebugInfo()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateInterval);

            UpdatePerformanceStats();
            UpdateDebugText();
        }
    }

    private void Update()
    {
        CalculateFrameStats();
    }

    private void CalculateFrameStats()
    {
        frameCount++;
        elapsedTime += Time.unscaledDeltaTime;

        if (elapsedTime >= updateInterval)
        {
            fps = frameCount / elapsedTime;
            frameTime = elapsedTime / frameCount * 1000f;

            frameCount = 0;
            elapsedTime = 0;
        }
    }

    private void UpdatePerformanceStats()
    {
        // Add more performance metrics here
    }

    private void UpdateDebugText()
    {
        stringBuilder.Clear();

        stringBuilder.AppendLine($"FPS: {fps:F1}");
        stringBuilder.AppendLine($"Frame Time: {frameTime:F2} ms");
        stringBuilder.AppendLine($"Memory Usage: {System.GC.GetTotalMemory(false) / (1024 * 1024):F2} MB");

        // Add more debug info as needed

        debugText.text = stringBuilder.ToString();
    }
}

#endif
