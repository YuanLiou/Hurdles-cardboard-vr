using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Line))]
public class LineInspector : Editor {

    private void OnSceneGUI() {
        Line line = target as Line;
        // set localTransform to worldTransform
        Transform handleTransform = line.transform;
        Vector3 worldPoint1 = handleTransform.TransformPoint(line.point1);
        Vector3 worldPoint2 = handleTransform.TransformPoint(line.point2);

        // get lines rotation
        Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

        Handles.color = Color.white;
        Handles.DrawLine(worldPoint1, worldPoint2);

        EditorGUI.BeginChangeCheck();
        worldPoint1 = Handles.DoPositionHandle(worldPoint1, handleRotation);
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(line, "Move Point");
            EditorUtility.SetDirty(line);
            line.point1 = handleTransform.InverseTransformPoint(worldPoint1);
        }

        EditorGUI.BeginChangeCheck();
        worldPoint2 = Handles.DoPositionHandle(worldPoint2, handleRotation);
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(line, "Move Point");
            EditorUtility.SetDirty(line);
            line.point2 = handleTransform.InverseTransformPoint(worldPoint2);
        }
    }

}