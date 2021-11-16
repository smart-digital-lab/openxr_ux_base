/**********************************************************************************************************************************************************
 * XRData_String
 * -------------
 *
 * 2021-11-16
 *
 * Editor Layer Settings for XRData_String
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRData_String
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRData_String))]
public class XRData_String_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRData_String myTarget = (XRData_String)target;

        XRUX_Editor_Settings.DrawMainHeading("String", "Generate an string of characters XRData event each time the Go trigger is activated.");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Go", "Trigger to activate function.", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawParametersHeading();
        myTarget.value = EditorGUILayout.TextField("Value to send", myTarget.value);
        myTarget.sendOnStart = EditorGUILayout.Toggle("Send when VE starts", myTarget.sendOnStart);

        XRUX_Editor_Settings.DrawOutputsHeading();
        var prop2 = serializedObject.FindProperty("onChange"); EditorGUILayout.PropertyField(prop2, true);    
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
