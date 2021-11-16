/**********************************************************************************************************************************************************
 * XRRig_SelectDeviceButton
 * ------------------------
 *
 * 2021-11-16
 *
 * Editor Layer Settings for XRRig_SelectDeviceButton
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRRig_SelectDeviceButton
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRRig_SelectDeviceButton))]
public class XRRig_SelectDeviceButton_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRRig_SelectDeviceButton myTarget = (XRRig_SelectDeviceButton)target;

        XRUX_Editor_Settings.DrawMainHeading("Select Device Buttons", "Change the color of this GameObject and possibly play a sound in response to an Event.");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Inputs from the controllers via the Event manager", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawParametersHeading();
        myTarget.eventToWatchFor = (XRDeviceEventTypes) EditorGUILayout.EnumPopup("Event to watch for", myTarget.eventToWatchFor);
        myTarget.actionToWatchFor = (XRDeviceActions) EditorGUILayout.EnumPopup("Action to watch for", myTarget.actionToWatchFor);
        myTarget.clickAudio = (AudioSource) EditorGUILayout.ObjectField("Sound to play (or none)", myTarget.clickAudio, typeof(Transform), true);
        myTarget.pressedColour = (Color) EditorGUILayout.ColorField("Color to change to", myTarget.pressedColour);

        XRUX_Editor_Settings.DrawOutputsHeading();
        EditorGUILayout.LabelField("Changing color of this GameObject and possible sound", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.Space();
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
