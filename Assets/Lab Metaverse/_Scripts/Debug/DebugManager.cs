using PG;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;

#if UNITY_EDITOR
public class DebugManager : MonoBehaviour
{
    [SerializeField] private TMP_Text debugText;
    [SerializeField] private float updateInterval = 0.5f;

    // add header for performance stats
    [Header("Stats")]
    public Transform playerTransform;
    public ControllerInput controllerInput;
    public CarController carController;

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

        stringBuilder.AppendLine($"Memory Usage: {System.GC.GetTotalMemory(false) / (1024 * 1024):F2} MB " +
            $"\tTotal Allocated Memory: {Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024):F2} MB");
        stringBuilder.AppendLine($"Total Unused: {Profiler.GetTotalUnusedReservedMemoryLong() / (1024 * 1024):F2} MB " + 
            $"\tTotal Reserved: {Profiler.GetTotalReservedMemoryLong() / (1024 * 1024):F2} MB\n");
        stringBuilder.AppendLine($"Player Position: {playerTransform.position}");

        // inputs
        stringBuilder.AppendLine($"Acceleration: {controllerInput.Acceleration:F3} \t\tBrake Reverse: {controllerInput.BrakeReverse:F3}");
        stringBuilder.AppendLine($"Horizontal: {controllerInput.Horizontal:F3} \t\t\tHandbrake: {controllerInput.HandBrake:F3}");
        stringBuilder.AppendLine($"Clutch: {controllerInput.Clutch:F3} \t\t\tCurrent Gear: {carController.CurrentGear}");

        // truck
        stringBuilder.AppendLine($"Engine RPM: {carController.EngineRPM:F3}");
        stringBuilder.AppendLine($"Current Motor Torque: {carController.CurrentMotorTorque:F3} \tWheel Torque: {carController.WheelTorque:F3}");

        // Add more debug info as needed

        debugText.text = stringBuilder.ToString();
    }
}

#endif
