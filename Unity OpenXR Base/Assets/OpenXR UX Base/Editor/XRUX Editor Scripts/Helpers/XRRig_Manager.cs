/**********************************************************************************************************************************************************
 * XRRig_Manager
 * -------------
 *
 * 2021-11-16
 *
 * Editor Layer Settings for XRRig_Manager
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRRig_Manager
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRRig_Manager))]
public class XRRig_Manager_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRRig_Manager myTarget = (XRRig_Manager)target;

        XRUX_Editor_Settings.DrawMainHeading("XRRig Manager", "The Smart Digital Lab OpenXR UX Base main Event Manager which translates the raw input signals from OpenXR and the mouse into standardized Events.");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Data from the OpenXR Controllers and other input devices", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawParametersHeading();

        XRUX_Editor_Settings.DrawOutputsHeading();
        EditorGUILayout.LabelField("Events on the event queue", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.Space();
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
