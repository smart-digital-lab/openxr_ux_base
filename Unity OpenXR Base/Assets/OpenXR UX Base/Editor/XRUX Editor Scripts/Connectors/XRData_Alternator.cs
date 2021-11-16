/**********************************************************************************************************************************************************
 * XRData_Alternator
 * -----------------
 *
 * 2021-11-15
 *
 * Editor Layer Settings for XRData_Alternator
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRData_Alternator
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRData_Alternator))]
public class XRData_Alternator_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRData_Alternator myTarget = (XRData_Alternator)target;

        XRUX_Editor_Settings.DrawMainHeading("Alternator", "Toggles output between true and false and sends an event each time the Go trigger is activated.");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Go", "Trigger to activate function.", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawParametersHeading();

        XRUX_Editor_Settings.DrawOutputsHeading();
        var prop2 = serializedObject.FindProperty("onChange"); EditorGUILayout.PropertyField(prop2, true);    
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
