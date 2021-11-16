/**********************************************************************************************************************************************************
 * XRUX_Keyboard
 * -------------
 *
 * 2021-11-15
 *
 * Editor Layer Settings for XRUX_Keyboard
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRUX_Keyboard
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRUX_Keyboard))]
public class XRUX_Keyboard_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRUX_Keyboard myTarget = (XRUX_Keyboard)target;

        XRUX_Editor_Settings.DrawMainHeading("Virtual Keyboard", "A keyboard for using in your Virtual Environments.");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Send", "Activate onSend.", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawParametersHeading();
        myTarget.clearOnSend = EditorGUILayout.Toggle("Clear on send.", myTarget.clearOnSend);
        myTarget.inputfield = (XRUX_Inputfield) EditorGUILayout.ObjectField("Input field object", myTarget.inputfield, typeof(XRUX_Inputfield), true);

        XRUX_Editor_Settings.DrawOutputsHeading();
        var prop2 = serializedObject.FindProperty("onSend"); EditorGUILayout.PropertyField(prop2, true);    
        EditorGUILayout.LabelField("Activates with the Enter key is pressed.", XRUX_Editor_Settings.helpTextStyle);
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
