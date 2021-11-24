/**********************************************************************************************************************************************************
 * XRRig_MakeVisible
 * -----------------
 *
 * 2021-11-16
 *
 * Editor Layer Settings for XRRig_MakeVisible
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRRig_MakeVisible
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRRig_MakeVisible))]
public class XRRig_MakeVisible_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRRig_MakeVisible myTarget = (XRRig_MakeVisible)target;

        XRUX_Editor_Settings.DrawMainHeading("Make Visible", "Make GameObject visible on given event and action.");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Input", "XRData", XRUX_Editor_Settings.fieldStyle);
        myTarget.activateOnEvent = EditorGUILayout.Toggle("Activate on OpenXR Event", myTarget.activateOnEvent);
        if (myTarget.activateOnEvent)
        {
            myTarget.eventToWatchFor = (XRDeviceEventTypes) EditorGUILayout.EnumPopup("Event to watch for", myTarget.eventToWatchFor);
            myTarget.actionToWatchFor = (XRDeviceActions) EditorGUILayout.EnumPopup("Action to watch for", myTarget.actionToWatchFor);
        }

        XRUX_Editor_Settings.DrawParametersHeading();

        XRUX_Editor_Settings.DrawOutputsHeading();
        EditorGUILayout.LabelField("GameObject visibility", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.Space();
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
