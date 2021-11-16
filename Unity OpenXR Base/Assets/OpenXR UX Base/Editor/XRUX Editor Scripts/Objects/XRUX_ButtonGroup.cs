/**********************************************************************************************************************************************************
 * XRUX_ButtonGroup
 * ----------------
 *
 * 2021-11-15
 *
 * Editor Layer Settings for XRUX_ButtonGroup
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRUX_ButtonGroup
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRUX_ButtonGroup))]
public class XRUX_ButtonGroup_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        XRUX_ButtonGroup myTarget = (XRUX_ButtonGroup)target;

        XRUX_Editor_Settings.DrawMainHeading("A Group of Buttons", "A set of buttons where one can be pressed at a time.  You can either specify the buttons by hand in Unity by creating buttons and adding them to the button group as children objects, or you can fill in the titles and have a list of buttons created dynamically at runtime.  Or both.");

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Input", "XRData", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.LabelField("Integer value to change which button is selected as if it was being pressed.", XRUX_Editor_Settings.helpTextStyle);

        XRUX_Editor_Settings.DrawParametersHeading();
        EditorGUILayout.LabelField("Dynamic Buttons, created from the prefab.", XRUX_Editor_Settings.categoryStyle);
        myTarget.buttonPrefab = (GameObject) EditorGUILayout.ObjectField("Button Prefab", myTarget.buttonPrefab, typeof(GameObject), true);
        myTarget.dynamicButtonSpacing = EditorGUILayout.FloatField("Dynamic Button Spacing", myTarget.dynamicButtonSpacing);
        EditorGUILayout.LabelField("Titles of Buttons to be created dynamically.  These will appear below the last child Button object.", XRUX_Editor_Settings.helpTextStyle);
        var prop = serializedObject.FindProperty("dynamicButtons"); EditorGUILayout.PropertyField(prop, true);    

        XRUX_Editor_Settings.DrawOutputsHeading();
        var prop2 = serializedObject.FindProperty("onChange"); EditorGUILayout.PropertyField(prop2, true);    
 
        serializedObject.ApplyModifiedProperties();
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
