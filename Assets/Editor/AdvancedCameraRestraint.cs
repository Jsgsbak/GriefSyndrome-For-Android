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

        Undo.RecordObject(cameraRestraint, "Ӧ�����λ��");

        if (GUILayout.Button("Ӧ�����λ��"))
        {
            cameraRestraint.CameraPoints[cameraRestraint.CameraPoints.Length - 1].Point = Camera.main.transform.position;
        }
    }

    
}
