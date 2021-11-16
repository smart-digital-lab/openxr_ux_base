/**********************************************************************************************************************************************************
 * XRData_Boolean
 * ---------------
 *
 * 2021-11-16
 *
 * Editor Layer Settings for XRData_Boolean
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRData_Boolean
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRData_Boolean))]
public class XRData_Boolean_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRData_Boolean myTarget = (XRData_Boolean)target;

        XRUX_Editor_Settings.DrawMainHeading("Boolean", "Generate a Boolean variable XRData event each time the Go trigger is activated.");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Go", "Trigger to activate function.", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawParametersHeading();
        myTarget.value = EditorGUILayout.Toggle("Value to send", myTarget.value);
        myTarget.sendOnStart = EditorGUILayout.Toggle("Send when VE starts", myTarget.sendOnStart);

        XRUX_Editor_Settings.DrawOutputsHeading();
        var prop2 = serializedObject.FindProperty("onChange"); EditorGUILayout.PropertyField(prop2, true);    
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
