/**********************************************************************************************************************************************************
 * XRData_Quietly
 * --------------
 *
 * 2021-11-16
 *
 * Editor Layer Settings for XRData_Quietly
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRData_Quietly
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRData_Quietly))]
public class XRData_Quietly_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRData_Quietly myTarget = (XRData_Quietly)target;

        XRUX_Editor_Settings.DrawMainHeading("Add Quietly parameter", "Add or removes the quietly parameter to XRData.  When sending XRData between XRUX entities, sometimes a feedback loop can occur, for example if you have two entities that need to be kept in synch with each other.  A feedback loop will completely crash Unity...  Adding a 'quietly' note to the XRData tells the receiving XRUX entity not to pass on the change any further, thus breaking the loop.");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Input", "XRData", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawParametersHeading();
        myTarget.quietly = EditorGUILayout.Toggle("New quietly value", myTarget.quietly);
        EditorGUILayout.LabelField("Set this to true to break a feedback loop.", XRUX_Editor_Settings.helpTextStyle);

        XRUX_Editor_Settings.DrawOutputsHeading();
        var prop2 = serializedObject.FindProperty("onChange"); EditorGUILayout.PropertyField(prop2, true);    
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
