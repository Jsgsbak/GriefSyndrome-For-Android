using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StopPoint))]
public class AdvancedStopPoints : Editor
{
    StopPoint stopPoint;

    private void Awake()
    {
        stopPoint = (StopPoint)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Undo.RecordObject(stopPoint, "应用相机位置");

        if (GUILayout.Button("应用相机位置"))
        {
            stopPoint.PointsEditor[stopPoint.PointsEditor.Count - 1] = Camera.main.transform.position;
        }

    }
}
