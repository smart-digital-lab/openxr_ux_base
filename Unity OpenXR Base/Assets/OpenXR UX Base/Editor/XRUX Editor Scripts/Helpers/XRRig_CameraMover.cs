/**********************************************************************************************************************************************************
 * XRRig_CameraMover
 * -----------------
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
// XRRig_CameraMover
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRRig_CameraMover))]
public class XRRig_CameraMover_Editor : Editor 
{
    public override void OnInspectorGUI()
    {
        XRRig_CameraMover myTarget = (XRRig_CameraMover)target;

        XRUX_Editor_Settings.DrawMainHeading("XRRig Camera Mover", "Manages the movement of the VR camera in Immersive and Desktop modes.");
        myTarget.mode = (XRData.Mode) EditorGUILayout.EnumPopup("Inspector Mode", myTarget.mode);

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Inputs from the XR Controllers", XRUX_Editor_Settings.fieldStyle);
        if (myTarget.mode == XRData.Mode.Advanced)
        {
            EditorGUILayout.LabelField("PutOnBrakes", XRUX_Editor_Settings.fieldStyle);
            EditorGUILayout.LabelField("StandOnGround", XRUX_Editor_Settings.fieldStyle);
            EditorGUILayout.LabelField("SetMovementStyle", "XRData", XRUX_Editor_Settings.fieldStyle);
        }

        XRUX_Editor_Settings.DrawParametersHeading();
        
        if (myTarget.mode == XRData.Mode.Advanced)
        {
            EditorGUILayout.LabelField("Connections into the XRRig", XRUX_Editor_Settings.categoryStyle);
            myTarget.leftMarker = (GameObject) EditorGUILayout.ObjectField("Left hand marker", myTarget.leftMarker, typeof(GameObject), true);
            myTarget.rightMarker = (GameObject) EditorGUILayout.ObjectField("Right hand marker", myTarget.rightMarker, typeof(GameObject), true);
            myTarget.leftPointer = (GameObject) EditorGUILayout.ObjectField("Left hand pointer", myTarget.leftPointer, typeof(GameObject), true);
            myTarget.rightPointer = (GameObject) EditorGUILayout.ObjectField("Right hand pointer", myTarget.rightPointer, typeof(GameObject), true);
            myTarget.theHead = (GameObject) EditorGUILayout.ObjectField("The head object", myTarget.theHead, typeof(GameObject), true);
            myTarget.thePlayer = (GameObject) EditorGUILayout.ObjectField("The Player object", myTarget.thePlayer, typeof(GameObject), true);
            myTarget.mainBody = (GameObject) EditorGUILayout.ObjectField("Main Body", myTarget.mainBody, typeof(GameObject), true);
            myTarget.instructions = (GameObject) EditorGUILayout.ObjectField("Instructions", myTarget.instructions, typeof(GameObject), true);
        }

        myTarget.height = EditorGUILayout.FloatField("Head height", myTarget.height);

        if (myTarget.mode == XRData.Mode.Advanced)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Teleportation", XRUX_Editor_Settings.categoryStyle);
        }
        myTarget.movementStyle = (XRRig_CameraMover.MovementStyle) EditorGUILayout.EnumPopup("Movement style", myTarget.movementStyle);
        if (myTarget.movementStyle == XRRig_CameraMover.MovementStyle.teleportToMarker)
        {
            if (myTarget.mode == XRData.Mode.Advanced)
            {
                myTarget.teleportFader = (GameObject) EditorGUILayout.ObjectField("The Player object", myTarget.teleportFader, typeof(GameObject), true);
            }
            myTarget.teleportFadeTime = EditorGUILayout.FloatField("Fade in and out time (s)", myTarget.teleportFadeTime);
        }
         
        if (myTarget.mode == XRData.Mode.Advanced)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Movement Control", XRUX_Editor_Settings.categoryStyle);
            myTarget.movementPointer = (XRRig_CameraMover.MovementDevice) EditorGUILayout.EnumPopup("Device to point for direction", myTarget.movementPointer);
            myTarget.movementController = (XRRig_CameraMover.MovementHand) EditorGUILayout.EnumPopup("Controller thumbstick for movement", myTarget.movementController);
            myTarget.otherThumbstickForHeight = EditorGUILayout.Toggle("Use other thumbstick for height", myTarget.otherThumbstickForHeight);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Movement Parameters", XRUX_Editor_Settings.categoryStyle);
            EditorGUILayout.LabelField("Acceleration is applied as long as the controller button is held, then friction slows down the movement.", XRUX_Editor_Settings.helpTextStyle);
            myTarget.accelerationFactor = EditorGUILayout.FloatField("Acceleration", myTarget.accelerationFactor);
            myTarget.frictionFactor = EditorGUILayout.FloatField("Friction", myTarget.frictionFactor);
            myTarget.maximumVelocity = EditorGUILayout.FloatField("Maximum velocity", myTarget.maximumVelocity);
            myTarget.maximumFlyingHeight = EditorGUILayout.FloatField("Maximum height to fly", myTarget.maximumFlyingHeight);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Rotation Parameters", XRUX_Editor_Settings.categoryStyle);
        }
        myTarget.rotationStyle = (XRRig_CameraMover.RotationStyle) EditorGUILayout.EnumPopup("Rotation style", myTarget.rotationStyle);
        if (myTarget.mode == XRData.Mode.Advanced)
        {
            if (myTarget.rotationStyle == XRRig_CameraMover.RotationStyle.Stepped) 
            { 
                myTarget.steppingAngle = EditorGUILayout.FloatField("Stepping angle (degrees)", myTarget.steppingAngle); 
            }
            else
            {
                EditorGUILayout.LabelField("Angular acceleration is applied as long as the controller button is held, then friction slows down the rotation.", XRUX_Editor_Settings.helpTextStyle);
                myTarget.rotationAccelerationFactor = EditorGUILayout.FloatField("Acceleration", myTarget.rotationAccelerationFactor);
                myTarget.rotationFrictionFactor = EditorGUILayout.FloatField("Friction", myTarget.rotationFrictionFactor);
            }
        }
        
        if (myTarget.mode == XRData.Mode.Advanced)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Dynamic Quality Settings", XRUX_Editor_Settings.categoryStyle);
            myTarget.sceneSettingsObjectName = EditorGUILayout.TextField("SceneSettings GameObject name", myTarget.sceneSettingsObjectName);
            EditorGUILayout.LabelField("If a scene contains a GameObject with this name, with the SceneSettings Component on it, those settings will override the ones below.", XRUX_Editor_Settings.helpTextStyle);
            myTarget.dynamicQuality = EditorGUILayout.Toggle("Change Quality Dynamically", myTarget.dynamicQuality);
            EditorGUILayout.LabelField("Default quality scenesettings if no GameObject found as per above.", XRUX_Editor_Settings.helpTextStyle);
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
        }

        EditorGUILayout.Space();
        XRUX_Editor_Settings.DrawOutputsHeading();
        EditorGUILayout.LabelField("Movement of the camera", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.Space();
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
