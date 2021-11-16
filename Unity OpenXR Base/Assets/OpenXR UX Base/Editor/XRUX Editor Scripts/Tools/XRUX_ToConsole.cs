/**********************************************************************************************************************************************************
 * XRUX_ToConsole
 * --------------
 *
 * 2021-11-15
 *
 * Editor Layer Settings for XRUX_ToConsole
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRUX_ToConsole
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRUX_ToConsole))]
public class XRUX_ToConsole_Editor : Editor 
{
    public override void OnInspectorGUI()
    {
        XRUX_Editor_Settings.DrawMainHeading("Send XRData to console", "Send inputted data to the Console via the event queue.");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Input", "XRData", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawParametersHeading();

        XRUX_Editor_Settings.DrawOutputsHeading();
        EditorGUILayout.LabelField("Event on event Queue", "XREvent", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.Space();
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
