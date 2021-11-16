/**********************************************************************************************************************************************************
 * XRUX_Rotate
 * -----------
 *
 * 2021-11-15
 *
 * Editor Layer Settings for XRUX_Rotate
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRUX_Rotate
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRUX_Rotate))]
public class XRUX_Rotate_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRUX_Rotate myTarget = (XRUX_Rotate)target;

        XRUX_Editor_Settings.DrawMainHeading("Rotate Object", "Rotate the object the script is on to the given angle on the Y-Axis.");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Input", "XRData", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawParametersHeading();
        myTarget.rotationSpeed = EditorGUILayout.FloatField("Rotation Speed", myTarget.rotationSpeed);

        XRUX_Editor_Settings.DrawOutputsHeading();
        EditorGUILayout.LabelField("Y-Axis Rotation", "float", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.Space();
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
