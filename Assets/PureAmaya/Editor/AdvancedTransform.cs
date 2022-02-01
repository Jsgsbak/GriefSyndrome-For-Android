using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform))]
public class AdvancedTransform:Editor
{
    //���л���ʹundo֧�ֳ���
    [SerializeField]
    Transform tr;

    private void Awake()
    {
        tr = (Transform)target;
    }

    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();

        Undo.RecordObject(tr, "Transform�޸�");

        tr.position = EditorGUILayout.Vector3Field("WorldPos", tr.position);
        tr.localPosition = EditorGUILayout.Vector3Field("LocalPos", tr.localPosition);

        EditorGUILayout.Space();

        tr.rotation = Quaternion.Euler(EditorGUILayout.Vector3Field("EulerWorldRot", tr.rotation.eulerAngles));
        tr.localRotation = Quaternion.Euler(EditorGUILayout.Vector3Field("EulerLocalRot", tr.localRotation.eulerAngles));

        EditorGUILayout.Space();
        EditorGUILayout.Vector3Field("LossyScale",tr.lossyScale);
        tr.localScale = EditorGUILayout.Vector3Field("LocalScale", tr.localScale);

    }
}
