/**********************************************************************************************************************************************************
 * XRUX_MaterialColor
 * ------------------
 *
 * 2021-11-15
 *
 * Editor Layer Settings for XRUX_MaterialColor
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRUX_MaterialColor
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRUX_MaterialColor))]
public class XRUX_MaterialColour_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRUX_Editor_Settings.DrawMainHeading("Change Material Color", "Change the material colour of the object the script is on.");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("InputR", "XRData", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.LabelField("InputG", "XRData", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.LabelField("InputB", "XRData", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.LabelField("InputA", "XRData", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawParametersHeading();

        XRUX_Editor_Settings.DrawOutputsHeading();
        EditorGUILayout.LabelField("Material color", "color", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.Space();
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
