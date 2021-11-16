/**********************************************************************************************************************************************************
 * XRUX_ActivateByProximity
 * ------------------------
 *
 * 2021-11-15
 *
 * Editor Layer Settings for XRUX_ActivateByProximity
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRUX_ActivateByProximity
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRUX_ActivateByProximity))]
public class XRUX_ActivateByProximity_Editor : Editor 
{
    public override void OnInspectorGUI()
    {
        XRUX_ActivateByProximity myTarget = (XRUX_ActivateByProximity)target;

        XRUX_Editor_Settings.DrawMainHeading("Activate by proximity", "Sends an event with the input data when the user comes close.  Only sends one event when the user first enters the area.");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Input", "XRData", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawParametersHeading();
        myTarget.distance = (float) EditorGUILayout.FloatField("Activation distance", myTarget.distance);

        XRUX_Editor_Settings.DrawOutputsHeading();
        myTarget.activateTrigger = (XRDeviceEventTypes) EditorGUILayout.EnumPopup("Event trigger to send", myTarget.activateTrigger);
        myTarget.activateAction = (XRDeviceActions) EditorGUILayout.EnumPopup("Event action to send", myTarget.activateAction);
        EditorGUILayout.Space();
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
