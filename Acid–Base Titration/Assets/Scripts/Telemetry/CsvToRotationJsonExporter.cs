using System;
using System.Collections.Generic;
using UnityEngine;

public class CsvToRotationJsonExporter : MonoBehaviour
{
    [Header("Input CSV")]
    public TextAsset csvFile;

    [Header("Telemetry Settings")]
    public float samplingRateHz = 100f;
    public string video_id = "Animation Data";

    [Header("Output (Editor Only)")]
    [Tooltip("Last saved JSON path (for reference)")]
    public string lastSavedPath;
}
[Serializable]
public class RotationFrame
{
    public string time;
    public float pitch;
    public float roll;
    public float yaw;
}

[Serializable]
public class TelemetryMetadata
{
    public string video_id = "Animation Data";
    public float duration_sec;
    public float sampling_rate_hz;
}

[Serializable]
public class RotationTelemetryJson
{
    public TelemetryMetadata metadata;
    public List<RotationFrame> frames;
}