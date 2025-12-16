#if UNITY_EDITOR
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CsvToRotationJsonExporter))]
public class CsvToRotationJsonExporterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CsvToRotationJsonExporter exporter =
            (CsvToRotationJsonExporter)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Generate JSON From CSV"))
        {
            ConvertCsvToJson(exporter);
        }
    }

    private void ConvertCsvToJson(CsvToRotationJsonExporter exporter)
    {
        if (!exporter.csvFile)
        {
            Debug.LogError("CSV file not assigned.");
            return;
        }

        // --- Ask user for save path ---
        string savePath = EditorUtility.SaveFilePanel(
            "Save Rotation Telemetry JSON",
            Application.dataPath,
            exporter.csvFile.name + "_telemetry",
            "json"
        );

        if (string.IsNullOrEmpty(savePath))
        {
            Debug.Log("JSON export cancelled.");
            return;
        }

        string[] lines = exporter.csvFile.text.Split('\n');

        RotationTelemetryJson json = new RotationTelemetryJson
        {
            metadata = new TelemetryMetadata
            {
                sampling_rate_hz = exporter.samplingRateHz
            },
            frames = new List<RotationFrame>()
        };

        float lastTimeMs = 0f;

        // Skip header
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] cols = lines[i].Split(',');

            float timeMs = float.Parse(cols[0], CultureInfo.InvariantCulture);
            float roll = float.Parse(cols[1], CultureInfo.InvariantCulture);
            float pitch = float.Parse(cols[2], CultureInfo.InvariantCulture);
            float yaw = float.Parse(cols[3], CultureInfo.InvariantCulture);

            json.frames.Add(new RotationFrame
            {
                time = (timeMs / 1000f).ToString("F4", CultureInfo.InvariantCulture),
                roll = roll,
                pitch = pitch,
                yaw = yaw
            });

            lastTimeMs = timeMs;
        }

        json.metadata.duration_sec = lastTimeMs / 1000f;
        json.metadata.video_id = exporter.video_id;
        string jsonText = JsonUtility.ToJson(json, true);
        File.WriteAllText(savePath, jsonText);

        exporter.lastSavedPath = savePath;
        EditorUtility.SetDirty(exporter);

        Debug.Log($"JSON generated at: {savePath}");
    }
}
#endif
