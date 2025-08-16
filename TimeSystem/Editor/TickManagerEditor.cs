using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Ticker))]
public class TickerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Ticker manager = (Ticker)target;

        // Draw the enableTickManager bool first
        manager.GetType()
            .GetField("enableTickManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(manager, EditorGUILayout.Toggle(
                new GUIContent("Enable Tick Manager", "Enable to allow TickManager to control tick timing."),
                manager.EnableTickManager));

        // Only show tickInterval if enableTickManager is true
        if (manager.EnableTickManager)
        {
            float newInterval = EditorGUILayout.FloatField(
                new GUIContent("Tick Interval (seconds)", "Time interval between ticks in seconds."),
                manager.TickInterval);

            if (newInterval != manager.TickInterval)
            {
                var intervalField = manager.GetType()
                    .GetField("tickInterval", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                intervalField.SetValue(manager, Mathf.Max(newInterval, 0.01f));
            }
        }

        // Mark dirty if anything changed
        if (GUI.changed)
            EditorUtility.SetDirty(manager);
    }
}
