/**********************************************************************************************************************************************************
 * XRUX_SetScene
 * -------------
 *
 * 2021-11-16
 *
 * Editor Layer Settings for XRUX_SetScene
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRUX_SetScene
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRUX_SetScene))]
public class XRUX_SetScene_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRUX_SetScene myTarget = (XRUX_SetScene)target;

        XRUX_Editor_Settings.DrawMainHeading("Set the Scene", "Allows the scene to be changed programmatically. This component should be placed on the same GameObject that has the XRRig Camera Mover component as it requires some of that functionality to operate correctly.");

        XRUX_Editor_Settings.DrawInputsHeading();
        // EditorGUILayout.LabelField("Settings to impose when the scene changes.", XRUX_Editor_Settings.categoryStyle);
        // EditorGUILayout.LabelField("Quality", "XRData", XRUX_Editor_Settings.fieldStyle);
        // EditorGUILayout.LabelField("Antialias", "XRData", XRUX_Editor_Settings.fieldStyle);
        // EditorGUILayout.LabelField("Texturelevel", "XRData", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.LabelField("Change the scene to the number given.", XRUX_Editor_Settings.categoryStyle);
        EditorGUILayout.LabelField("Input", "XRData", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.LabelField("Global XREvents", "scene | CHANGE, with number", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawParametersHeading();
        myTarget.startScene = EditorGUILayout.IntField("Scene to start with (-1 for this scene)", myTarget.startScene);
        myTarget.persistAcrossScenes = EditorGUILayout.Toggle("Persist this XRRig across scenes.", myTarget.persistAcrossScenes);

        XRUX_Editor_Settings.DrawOutputsHeading();
        var prop2 = serializedObject.FindProperty("onChange"); EditorGUILayout.PropertyField(prop2, true);    
        EditorGUILayout.Space();
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
