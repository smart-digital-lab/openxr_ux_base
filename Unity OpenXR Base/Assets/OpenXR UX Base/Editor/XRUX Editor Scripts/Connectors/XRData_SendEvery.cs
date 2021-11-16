/**********************************************************************************************************************************************************
 * XRData_SendEvery
 * ----------------
 *
 * 2021-11-16
 *
 * Editor Layer Settings for XRData_SendEvery
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRData_SendEvery
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRData_SendEvery))]
public class XRData_SendEvery_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRData_SendEvery myTarget = (XRData_SendEvery)target;

        XRUX_Editor_Settings.DrawMainHeading("Send Every", "Send an event regularly.  This can be used as a trigger to cause other events to occur.");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Go", "Start sending", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.LabelField("Stop", "Stop sending", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawParametersHeading();
        myTarget.timeInSeconds = EditorGUILayout.FloatField("Time between events (s)", myTarget.timeInSeconds);

        XRUX_Editor_Settings.DrawOutputsHeading();
        var prop2 = serializedObject.FindProperty("onChange"); EditorGUILayout.PropertyField(prop2, true);    
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
