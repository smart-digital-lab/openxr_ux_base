/**********************************************************************************************************************************************************
 * XRData_From
 * -----------
 *
 * 2021-11-16
 *
 * Editor Layer Settings for XRData_From
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRData_From
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRData_From))]
public class XRData_From_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRData_From myTarget = (XRData_From)target;

        XRUX_Editor_Settings.DrawMainHeading("Convert from XRData", "Convert XRData into one of its component data types.");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Input", "XRData", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawParametersHeading();

        XRUX_Editor_Settings.DrawOutputsHeading();
        var prop = serializedObject.FindProperty("onChangeBoolean"); EditorGUILayout.PropertyField(prop, true);    
        var prop2 = serializedObject.FindProperty("onChangeInteger"); EditorGUILayout.PropertyField(prop2, true);    
        var prop3 = serializedObject.FindProperty("onChangeFloat"); EditorGUILayout.PropertyField(prop3, true);    
        var prop4 = serializedObject.FindProperty("onChangeString"); EditorGUILayout.PropertyField(prop4, true);   
        var prop5 = serializedObject.FindProperty("onChangeVector3"); EditorGUILayout.PropertyField(prop5, true);   
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
