/**********************************************************************************************************************************************************
 * XRUX_Console
 * ------------
 *
 * 2021-11-15
 *
 * Editor Layer Settings for XRUX_Console
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRUX_Console
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRUX_Console))]
public class XRUX_Console_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRUX_Console myTarget = (XRUX_Console)target;

        XRUX_Editor_Settings.DrawMainHeading("Debugging Console", "A console-like text field that holds a number of lines of text.  New text goes on the bottom, and the rest shifts up until it disappears off the top.  The console can accept global XREvents (sent via the ToConsole module).");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Input", "string | int | float | bool | Vector3 | XRData", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.LabelField("Clear", "Clear the console.", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.LabelField("Global XREvents", "console | CHANGE", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawParametersHeading();
        myTarget.numLines = EditorGUILayout.IntField("Number of lines", myTarget.numLines);
        myTarget.acceptGlobal = EditorGUILayout.Toggle("Accept global XREvents", myTarget.acceptGlobal);

        XRUX_Editor_Settings.DrawOutputsHeading();
        EditorGUILayout.LabelField("Actions", "Put text onto console.", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
