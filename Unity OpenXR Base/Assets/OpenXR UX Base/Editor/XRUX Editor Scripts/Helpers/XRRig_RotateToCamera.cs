/**********************************************************************************************************************************************************
 * XRRig_RotateToCamera
 * --------------------
 *
 * 2021-11-16
 *
 * Editor Layer Settings for XRRig_RotateToCamera
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRRig_RotateToCamera
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRRig_RotateToCamera))]
public class XRRig_RotateToCamera_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRRig_RotateToCamera myTarget = (XRRig_RotateToCamera)target;

        XRUX_Editor_Settings.DrawMainHeading("Rotate to Camera", "Rotate the object this component is on towards the direction the camera is pointing in Immersive VR.  This is intended to be used to rotate objects on the XRRig Head to keep them in view and allows for a certain amount of head movement before rotating to keep up.");

        XRUX_Editor_Settings.DrawInputsHeading();
        myTarget.theCamera = (Transform) EditorGUILayout.ObjectField("Camera GameObject", myTarget.theCamera, typeof(Transform), true);

        XRUX_Editor_Settings.DrawParametersHeading();

        XRUX_Editor_Settings.DrawOutputsHeading();
        EditorGUILayout.LabelField("Rotation of this GameObject on the y axis", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.Space();
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
