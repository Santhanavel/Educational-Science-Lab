#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;

[CustomEditor(typeof(AnimationClipCsvExporter))]
public class AnimationClipCsvExporterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AnimationClipCsvExporter exporter = (AnimationClipCsvExporter)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Generate CSV"))
        {
            ExportCSV(exporter);
        }
    }
    private void ExportCSV(AnimationClipCsvExporter exporter)
    {
        AnimationClip clip = exporter.animationClip;
        if (!clip)
        {
            Debug.LogError("AnimationClip is not assigned.");
            return;
        }

        // Euler curves
        AnimationCurve ex = null, ey = null, ez = null;

        foreach (var binding in AnimationUtility.GetCurveBindings(clip))
        {
            string p = binding.propertyName;

            if (p == "localEulerAnglesRaw.x") ex = AnimationUtility.GetEditorCurve(clip, binding);
            else if (p == "localEulerAnglesRaw.y") ey = AnimationUtility.GetEditorCurve(clip, binding);
            else if (p == "localEulerAnglesRaw.z") ez = AnimationUtility.GetEditorCurve(clip, binding);
        }

        if (ex == null || ey == null || ez == null)
        {
            Debug.LogError("Euler rotation curves not found in AnimationClip.");
            return;
        }

        const float sampleRateHz = 100f;       // 100 values per second
        const float stepMs = 10f;               // 10 ms
        float durationMs = clip.length * 1000f;

        int totalSamples = Mathf.RoundToInt(durationMs / stepMs);

        StringBuilder csv = new StringBuilder();
        csv.AppendLine("time_ms,roll,pitch,yaw");

        for (int i = 0; i <= totalSamples; i++)
        {
            float timeMs = i * stepMs;
            float timeSec = timeMs / 1000f;

            Vector3 euler = new Vector3(
                ex.Evaluate(timeSec),
                ey.Evaluate(timeSec),
                ez.Evaluate(timeSec)
            );

            csv.AppendLine(
                $"{timeMs:F0}," +
                $"{euler.x:F3}," +
                $"{euler.y:F3}," +
                $"{euler.z:F3}"
            );
        }

        string path = Path.Combine(
            Application.dataPath,
            exporter.fileName + "_Euler_100Hz.csv"
        );

        File.WriteAllText(path, csv.ToString());
        AssetDatabase.Refresh();

        Debug.Log($"Euler 100Hz CSV generated: {path}");
    }
}
#endif
