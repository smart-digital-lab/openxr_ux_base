/**********************************************************************************************************************************************************
 * XRUX_SceneSettings
 * ------------------
 *
 * 2021-11-15
 *
 * Editor Layer Settings for XRUX_SceneSettings
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRUX_SceneSettings
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRUX_SceneSettings))]
public class XRUX_SceneSettings_Editor : Editor 
{
    public override void OnInspectorGUI()
    {
        XRUX_SceneSettings myTarget = (XRUX_SceneSettings)target;

        XRUX_Editor_Settings.DrawMainHeading("Dynamic Scene Settings", "Contains data to be used by XRRig_CameraMover on scene load about the visual quality settings.  XRRig_CameraMover is on the root of the default OpenXRUX camera rig.  The settings below will override the settings on the XRRig_CameraMover script and is useful for having different quality settings for different scenes.  This script must be placed on the GameObject with the name that the XRRig_CameraMover script refers to (default is \"ENTRY\")");

        XRUX_Editor_Settings.DrawInputsHeading();

        XRUX_Editor_Settings.DrawParametersHeading();
        myTarget.dynamicQuality = EditorGUILayout.Toggle("Change Quality Dynamically", myTarget.dynamicQuality);
        EditorGUILayout.Space();

        if (myTarget.dynamicQuality)
        {
            EditorGUILayout.LabelField("When moving", XRUX_Editor_Settings.categoryStyle);
            myTarget.movingAntiAliasingLevel = (SceneSettingsAntiAliasing) EditorGUILayout.EnumPopup("Antialiasing level", myTarget.movingAntiAliasingLevel);
            myTarget.movingTextureQuality = (SceneSettingsTextureQuality) EditorGUILayout.EnumPopup("Texture Quality", myTarget.movingTextureQuality);
            myTarget.movingVisualQuality = (SceneSettingsVisualQuality) EditorGUILayout.EnumPopup("Visual Quality", myTarget.movingVisualQuality);
            myTarget.movingShadowQuality = (ShadowQuality) EditorGUILayout.EnumPopup("Shadow Quality", myTarget.movingShadowQuality);
            myTarget.movingShadowResolution = (ShadowResolution) EditorGUILayout.EnumPopup("Shadow Resolution", myTarget.movingShadowResolution);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("When stationary", XRUX_Editor_Settings.categoryStyle);
            myTarget.standingAntiAliasingLevel = (SceneSettingsAntiAliasing) EditorGUILayout.EnumPopup("Antialiasing level", myTarget.standingAntiAliasingLevel);
            myTarget.standingTextureQuality = (SceneSettingsTextureQuality) EditorGUILayout.EnumPopup("Texture Quality", myTarget.standingTextureQuality);
            myTarget.standingVisualQuality = (SceneSettingsVisualQuality) EditorGUILayout.EnumPopup("Visual Quality", myTarget.standingVisualQuality);
            myTarget.standingShadowQuality = (ShadowQuality) EditorGUILayout.EnumPopup("Shadow Quality", myTarget.standingShadowQuality);
            myTarget.standingShadowResolution = (ShadowResolution) EditorGUILayout.EnumPopup("Shadow Resolution", myTarget.standingShadowResolution);
        }

        XRUX_Editor_Settings.DrawOutputsHeading();
        EditorGUILayout.Space();
        
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
