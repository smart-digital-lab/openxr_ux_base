/**********************************************************************************************************************************************************
 * XRData_RGBToHex
 * ---------------
 *
 * 2021-11-16
 *
 * Editor Layer Settings for XRData_RGBToHex
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRData_RGBToHex
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRData_RGBtoHex))]
public class XRData_RGBToHex_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRData_RGBtoHex myTarget = (XRData_RGBtoHex)target;

        XRUX_Editor_Settings.DrawMainHeading("Convert RGB to Hex", "Convert individual Red, Green, Blue and Alpha values to an eight digit Hexadecimal string.");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("InputR", "XRData - red", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.LabelField("InputG", "XRData - green", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.LabelField("InputB", "XRData - blue", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.LabelField("InputA", "XRData - alpha", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawParametersHeading();

        XRUX_Editor_Settings.DrawOutputsHeading();
        var prop2 = serializedObject.FindProperty("onChange"); EditorGUILayout.PropertyField(prop2, true);    
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
