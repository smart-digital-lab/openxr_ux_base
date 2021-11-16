/**********************************************************************************************************************************************************
 * XRRig_SendEventOnAngle
 * ----------------------
 *
 * 2021-11-16
 *
 * Editor Layer Settings for XRRig_SendEventOnAngle
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRRig_SendEventOnAngle
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRRig_SendEventOnAngle))]
public class XRRig_SendEventOnAngle_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRRig_SendEventOnAngle myTarget = (XRRig_SendEventOnAngle)target;

        XRUX_Editor_Settings.DrawMainHeading("Send Event on Angle", "Send a chosen event and action, and boolean value, when the two transformations have less than a certain angle between them.  Sends true when going inside the angle and false when going outside of the angle.");

        XRUX_Editor_Settings.DrawInputsHeading();
        myTarget.source = (Transform) EditorGUILayout.ObjectField("Source transformation", myTarget.source, typeof(Transform), true);
        myTarget.target = (Transform) EditorGUILayout.ObjectField("Target transformation", myTarget.target, typeof(Transform), true);

        XRUX_Editor_Settings.DrawParametersHeading();
        myTarget.triggerAngle = EditorGUILayout.FloatField("Angle at which to trigger", myTarget.triggerAngle);
        myTarget.zDirection = (XRRig_SendEventOnAngle.Direction) EditorGUILayout.EnumPopup("Look forwards or backwards", myTarget.zDirection);

        XRUX_Editor_Settings.DrawOutputsHeading();
        var prop = serializedObject.FindProperty("eventToSendOnTrigger"); EditorGUILayout.PropertyField(prop, true);    
        var prop2 = serializedObject.FindProperty("actionToSendOnTrigger"); EditorGUILayout.PropertyField(prop2, true);    

        EditorGUILayout.Space();
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
