/**********************************************************************************************************************************************************
 * XRUX_SetText
 * ------------
 *
 * 2021-11-15
 *
 * Editor Layer Settings for XRUX_SetText
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRUX_SetText
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRUX_SetText))]
public class XRUX_SetText_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRUX_Editor_Settings.DrawMainHeading("Set Text", "Set the TextMeshPro text to the given input.  Place on an object that has a TextMeshPro component.");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Input", "string | int | float | bool | Vector3 | XRData", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawParametersHeading();

        XRUX_Editor_Settings.DrawOutputsHeading();
        EditorGUILayout.LabelField("Text on the GameObject", "string", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.Space();
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
