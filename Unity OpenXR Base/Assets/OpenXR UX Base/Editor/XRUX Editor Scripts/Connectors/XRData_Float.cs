/**********************************************************************************************************************************************************
 * XRData_Float
 * ------------
 *
 * 2021-11-16
 *
 * Editor Layer Settings for XRData_Float
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRData_Float
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRData_Float))]
public class XRData_Float_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRData_Float myTarget = (XRData_Float)target;

        XRUX_Editor_Settings.DrawMainHeading("Float", "Generate a Floating Point XRData event each time the Go trigger is activated.");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Go", "Trigger to activate function.", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawParametersHeading();
        myTarget.value = EditorGUILayout.FloatField("Value to send", myTarget.value);
        myTarget.sendOnStart = EditorGUILayout.Toggle("Send when VE starts", myTarget.sendOnStart);

        XRUX_Editor_Settings.DrawOutputsHeading();
        var prop2 = serializedObject.FindProperty("onChange"); EditorGUILayout.PropertyField(prop2, true);    
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
