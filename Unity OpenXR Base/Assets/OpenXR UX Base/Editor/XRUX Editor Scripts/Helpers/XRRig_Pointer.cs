/**********************************************************************************************************************************************************
 * XRRig_Pointer
 * -------------
 *
 * 2021-11-16
 *
 * Editor Layer Settings for XRRig_Pointer
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRRig_Pointer
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRRig_Pointer))]
public class XRRig_Pointer_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRRig_Pointer myTarget = (XRRig_Pointer)target;

        XRUX_Editor_Settings.DrawMainHeading("Pointer", "Detects when the user is pointing at clickable objects and places to move to, and draws either a curved arc or a line connecting the controllers and the place being pointed to.  Should go on the pointer object attached to the left and right Controllers.");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Inputs from the controllers via the Event manager", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawParametersHeading();
        EditorGUILayout.LabelField("Clickable GameObjects either for interacting with or moving to", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawOutputsHeading();
        EditorGUILayout.LabelField("Activation of the GameObject, or movement of the camera", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.Space();
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
