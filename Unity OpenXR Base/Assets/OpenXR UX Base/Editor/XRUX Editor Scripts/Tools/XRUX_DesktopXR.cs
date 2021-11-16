/**********************************************************************************************************************************************************
 * XRUX_DesktopXR
 * --------------
 *
 * 2021-11-15
 *
 * Editor Layer Settings for XRUX_DesktopXR
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRUX_DesktopVR
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRUX_DesktopXR))]
public class XRUX_DesktopXR_Editor : Editor 
{
    public override void OnInspectorGUI()
    {
        XRUX_DesktopXR myTarget = (XRUX_DesktopXR)target;

        XRUX_Editor_Settings.DrawMainHeading("Desktop / Immersive XR Switch", "Activate the object in Desktop or Immersive XR mode.");

        // XRUX_Editor_Settings.DrawInputsHeading();

        XRUX_Editor_Settings.DrawParametersHeading();
        myTarget.activateForTypeOfXR = (XRUX_DesktopXR.XRType) EditorGUILayout.EnumPopup("Activation mode", myTarget.activateForTypeOfXR);

        // XRUX_Editor_Settings.DrawOutputsHeading();
        // EditorGUILayout.LabelField("Activate", "bool", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.Space();
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
