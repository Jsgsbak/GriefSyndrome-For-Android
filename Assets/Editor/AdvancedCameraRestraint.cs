using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraRestraint))]
public class AdvancedCameraRestraint:Editor
{
    [SerializeField]
    CameraRestraint cameraRestraint;

    private void Awake()
    {
        cameraRestraint = (CameraRestraint)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Undo.RecordObject(cameraRestraint, "应用相机位置");

        if (GUILayout.Button("应用相机位置"))
        {
            cameraRestraint.CameraPoints[^1].Point = Camera.main.transform.position;
        }
    }

    
}
