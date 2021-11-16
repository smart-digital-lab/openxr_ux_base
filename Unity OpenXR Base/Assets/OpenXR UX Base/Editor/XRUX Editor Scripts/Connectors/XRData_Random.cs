/**********************************************************************************************************************************************************
 * XRData_Random
 * -------------
 *
 * 2021-11-16
 *
 * Editor Layer Settings for XRData_Random
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRData_Random
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRData_Random))]
public class XRData_Random_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRData_Random myTarget = (XRData_Random)target;

        XRUX_Editor_Settings.DrawMainHeading("Random", "Generate a random number between the minimum and maximum values.");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Go", "Trigger to activate function.", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawParametersHeading();
        myTarget.minValue = EditorGUILayout.FloatField("Minimum value", myTarget.minValue);
        myTarget.maxValue = EditorGUILayout.FloatField("Maximum Value", myTarget.maxValue);
        myTarget.sendOnStart = EditorGUILayout.Toggle("Send when VE starts", myTarget.sendOnStart);

        XRUX_Editor_Settings.DrawOutputsHeading();
        var prop2 = serializedObject.FindProperty("onChange"); EditorGUILayout.PropertyField(prop2, true);    
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
