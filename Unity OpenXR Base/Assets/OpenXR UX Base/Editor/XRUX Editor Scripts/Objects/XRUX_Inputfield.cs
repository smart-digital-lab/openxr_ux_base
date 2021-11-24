/**********************************************************************************************************************************************************
 * XRUX_Inputfield
 * ---------------
 *
 * 2021-11-15
 *
 * Editor Layer Settings for XRUX_Inputfield
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRUX_Inputfield
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRUX_Inputfield))]
public class XRUX_InputField_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRUX_Inputfield myTarget = (XRUX_Inputfield)target;
        myTarget.mode = (XRData.Mode) EditorGUILayout.EnumPopup("Inspector Mode", myTarget.mode);

        XRUX_Editor_Settings.DrawMainHeading("Input Field", "An XR UX element that collects text as if from a keyboard.");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Backspace", "Delete the last character.", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.LabelField("Capslock", "Turn on Capslock.", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.LabelField("Shiftlock", "Turn on Shiftlock.", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.LabelField("Clear", "Clear the display.", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.LabelField("Input", "string | int | float | bool | Vector3 | XRData", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.LabelField("Input", "XRData (bool), string - put the string only if XRdata is true", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.LabelField("Send", "Send the collated text to the onSend event queue.", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawParametersHeading();
        myTarget.singleCharacters = EditorGUILayout.Toggle("Single characters only", myTarget.singleCharacters);
        EditorGUILayout.LabelField("When true, the first of two characters sent as a string will be used with shiftlock off, and the second with shiftlock on.", XRUX_Editor_Settings.helpTextStyle);

        XRUX_Editor_Settings.DrawOutputsHeading();
        var prop2 = serializedObject.FindProperty("onSend"); EditorGUILayout.PropertyField(prop2, true);    
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
