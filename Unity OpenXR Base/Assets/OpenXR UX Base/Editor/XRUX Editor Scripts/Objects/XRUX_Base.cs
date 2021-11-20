/**********************************************************************************************************************************************************
 * XRUX_Base
 * ---------
 *
 * 2021-11-15
 *
 * Editor Layer Settings for XRUX_Base
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;
using TMPro;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRUX_Base
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRUX_Base))]
public class XRUX_Base_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRUX_Base myTarget = (XRUX_Base)target;
        myTarget.mode = (XRData.Mode) EditorGUILayout.EnumPopup("Inspector Mode", myTarget.mode);

        XRUX_Editor_Settings.DrawMainHeading("Minimisable surface for XRUX Objects", "A flat surface to put XR Objects on that can be minimised and maximised.  Change the shape of the base object for non-flat UXs.  Primarily used by the XRUX_Base prefab.");
      
        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Maximise", "Input from other Script.", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.LabelField("Minimise", "Input from other Script.", XRUX_Editor_Settings.fieldStyle);

        XRUX_Editor_Settings.DrawParametersHeading();
        myTarget.startMinimised = EditorGUILayout.Toggle("Start Minimised", myTarget.startMinimised);
        myTarget.Maximised = (GameObject) EditorGUILayout.ObjectField("GameObject to show when maximised.", myTarget.Maximised, typeof(GameObject), true);
        myTarget.Minimised = (GameObject) EditorGUILayout.ObjectField("GameObject to show when minimised.", myTarget.Minimised, typeof(GameObject), true);

        XRUX_Editor_Settings.DrawOutputsHeading();
        EditorGUILayout.LabelField("Actions", "Activate or inactivate GameObjects in the hierarchy.", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.Space();
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
