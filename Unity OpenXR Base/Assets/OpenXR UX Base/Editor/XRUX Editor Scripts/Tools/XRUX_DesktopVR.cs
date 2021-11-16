/**********************************************************************************************************************************************************
 * XRUX_DesktopVR
 * --------------
 *
 * 2021-11-15
 *
 * Editor Layer Settings for XRUX_DesktopVR
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRUX_DesktopVR
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRUX_DesktopVR))]
public class XRUX_DesktopVR_Editor : Editor 
{
    public override void OnInspectorGUI()
    {
        XRUX_DesktopVR myTarget = (XRUX_DesktopVR)target;

        XRUX_Editor_Settings.DrawMainHeading("Desktop / Immersive VR Switch", "Activate the object in Desktop or Immersive VR mode.");

        XRUX_Editor_Settings.DrawInputsHeading();

        XRUX_Editor_Settings.DrawParametersHeading();
        myTarget.activateForTypeOfXR = (XRUX_DesktopVR.XRType) EditorGUILayout.EnumPopup("Activate in", myTarget.activateForTypeOfXR);

        XRUX_Editor_Settings.DrawOutputsHeading();
        EditorGUILayout.LabelField("Activate", "bool", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.Space();
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
