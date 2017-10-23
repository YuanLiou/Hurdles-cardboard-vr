using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierSpline))]
public class BeziersplineInspector : Editor {

    private static Color[] MODE_COLORS = {
        Color.white,
        Color.yellow,
        Color.cyan
    };

    private BezierSpline spline;
    private Transform handleTransform;
    private Quaternion handleRotation;

    private const int stepsPerCurve = 10;
    private const float directionScale = 0.5f;
    private const float handleSize = 0.04f;
    private const float pickSize = 0.06f;

    private int selectedIndex = -1;

    private void OnSceneGUI() {
        spline = target as BezierSpline;
        handleTransform = spline.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local
            ? handleTransform.rotation
            : Quaternion.identity;
        Vector3 p0 = ShowPoint(0);

        for (int i = 1; i < spline.ControlPointsCount; i += 3) {
            Vector3 p1 = ShowPoint(i);
            Vector3 p2 = ShowPoint(i + 1);
            Vector3 p3 = ShowPoint(i + 2);

            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);

            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
            p0 = p3;
        }

        ShowDirections();
    }

    public override void OnInspectorGUI() {
        spline = target as BezierSpline;
        EditorGUI.BeginChangeCheck();
        bool loop = EditorGUILayout.Toggle("Loop", spline.Loop);
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(spline, "Toggle Loop");
            EditorUtility.SetDirty(spline);
            spline.Loop = loop;
        }

        if (selectedIndex >= 0 && selectedIndex < spline.ControlPointsCount) {
            DrawSeletedPointInspector();
        }

        if (GUILayout.Button("Add Curve")) {
            Undo.RecordObject(spline, "Add Curve");
            EditorUtility.SetDirty(spline);
            spline.AddCurve();
        }

        if (GUILayout.Button("Reset")) {
            Undo.RecordObject(spline, "Reset Spline");
            EditorUtility.SetDirty(spline);
            spline.Reset();
        }
    }

    private Vector3 ShowPoint(int index) {
        Vector3 point = handleTransform.TransformPoint(spline.GetControlPoint(index));
        float size = HandleUtility.GetHandleSize(point);
        if (index == 0) {
            size *= 2f;
        }

        Handles.color = MODE_COLORS[(int) spline.getControlPointMode(index)];
        if (Handles.Button(point, handleRotation, handleSize * size, pickSize * size, Handles.DotHandleCap)) {
            selectedIndex = index;
            Repaint();    // fix inspector doesn't refresh issue.
        }

        if (selectedIndex == index) {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(spline, "Move Point");
                EditorUtility.SetDirty(spline);
                spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
            }
        }
        return point;
    }

    private void ShowDirections() {
        Handles.color = Color.green;
        Vector3 point = spline.GetPoint(0f);
        Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);
        int steps = stepsPerCurve * spline.CurveCount;
        for (int i = 1; i < steps; i++) {
            point = spline.GetPoint(i / (float) steps);
            Handles.DrawLine(point, point + spline.GetDirection(i / (float) steps) * directionScale);
        }
    }

    private void DrawSeletedPointInspector() {
        GUILayout.Label("Selected Point");
        EditorGUI.BeginChangeCheck();
        Vector3 point = EditorGUILayout.Vector3Field("position", spline.GetControlPoint(selectedIndex));
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(spline, "Move Point");
            EditorUtility.SetDirty(spline);
            spline.SetControlPoint(selectedIndex, point);
        }

        EditorGUI.BeginChangeCheck();
        BezierControlPointMode mode = (BezierControlPointMode) EditorGUILayout.EnumPopup("Mode", spline.getControlPointMode(selectedIndex));
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(spline, "Change Point Mode");
            EditorUtility.SetDirty(spline);
            spline.SetControlPointMode(selectedIndex, mode);
        }
    }
}