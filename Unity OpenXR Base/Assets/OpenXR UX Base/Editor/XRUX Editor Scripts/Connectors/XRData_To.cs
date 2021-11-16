/**********************************************************************************************************************************************************
 * XRData_To
 * ---------
 *
 * 2021-11-16
 *
 * Editor Layer Settings for XRData_To
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRData_To
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRData_To))]
public class XRData_To_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRData_To myTarget = (XRData_To)target;

        XRUX_Editor_Settings.DrawMainHeading("Convert to XRData", "Convert to XRData from one of its component data types.");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Input", "bool | float | int | string | Vector3", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawParametersHeading();

        XRUX_Editor_Settings.DrawOutputsHeading();
        var prop = serializedObject.FindProperty("onChange"); EditorGUILayout.PropertyField(prop, true);    
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
