/**********************************************************************************************************************************************************
 * XRUX_Knob
 * ---------
 *
 * 2021-11-15
 *
 * Editor Layer Settings for XRUX_Knob
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRUX_Knob
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRUX_Knob))]
public class XRUX_Knob_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRUX_Knob myTarget = (XRUX_Knob)target;
        myTarget.mode = (XRData.Mode) EditorGUILayout.EnumPopup("Inspector Mode", myTarget.mode);

        XRUX_Editor_Settings.DrawMainHeading("A Turnable Knob", "A knob is a turnable object that can be rotated by activating and twisting with the hand controller, or hovering over and using the scroll button on the mouse.");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Input", "XRData", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.LabelField("Float XRData value to change the rotation of the knob just as if it was being turned by hand.", XRUX_Editor_Settings.helpTextStyle);

        XRUX_Editor_Settings.DrawParametersHeading();
        if (myTarget.mode == XRData.Mode.Advanced)
        {
            EditorGUILayout.LabelField("The object that will change colour when touched and rotated.", XRUX_Editor_Settings.categoryStyle);
            myTarget.objectToColor = (Renderer) EditorGUILayout.ObjectField("Object to color", myTarget.objectToColor, typeof(Renderer), true);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("The object that will move when touched and rotated.", XRUX_Editor_Settings.categoryStyle);
            myTarget.objectToMove = (GameObject) EditorGUILayout.ObjectField("Object to move", myTarget.objectToMove, typeof(GameObject), true);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Materials for the interaction stages.", XRUX_Editor_Settings.categoryStyle);
            myTarget.normalMaterial = (Material) EditorGUILayout.ObjectField("Normal Material", myTarget.normalMaterial, typeof(Material), true);
            myTarget.activatedMaterial = (Material) EditorGUILayout.ObjectField("Activated Material", myTarget.activatedMaterial, typeof(Material), true);
            myTarget.touchedMaterial = (Material) EditorGUILayout.ObjectField("Touched Material", myTarget.touchedMaterial, typeof(Material), true);
            EditorGUILayout.Space();
        }
        EditorGUILayout.LabelField("Maximum value for the 360 degree turn, and size of steps.", XRUX_Editor_Settings.categoryStyle);
        myTarget.maxValue = EditorGUILayout.FloatField("Maximum Value", myTarget.maxValue);
        myTarget.step = EditorGUILayout.FloatField("Step", myTarget.step);

        EditorGUILayout.Space();
        XRUX_Editor_Settings.DrawOutputsHeading();
        var prop = serializedObject.FindProperty("onChange"); EditorGUILayout.PropertyField(prop, true); 
        if (myTarget.mode == XRData.Mode.Advanced)
        {
            var prop2 = serializedObject.FindProperty("onTouch"); EditorGUILayout.PropertyField(prop2, true);    
            var prop3 = serializedObject.FindProperty("onUntouch"); EditorGUILayout.PropertyField(prop3, true); 
        }   
        EditorGUILayout.Space();
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
