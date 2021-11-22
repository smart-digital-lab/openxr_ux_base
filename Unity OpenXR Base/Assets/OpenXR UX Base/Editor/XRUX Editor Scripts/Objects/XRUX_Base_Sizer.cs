/**********************************************************************************************************************************************************
 * XRUX_Base_Sizer
 * ---------------
 *
 * 2021-11-15
 *
 * Editor Layer Settings for XRUX_Base_Sizer
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;
using TMPro;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRUX_Base_Sizer
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRUX_Base_Sizer))]
public class XRUX_Base_Sizer_Editor : Editor
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    float width, height, thickness;
    XRUX_Base_Sizer mainTarget;
    XRUX_Base myTarget;
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set up certain values when enabled
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void OnEnable()
    {
        mainTarget = (XRUX_Base_Sizer)target;
        myTarget = (XRUX_Base)mainTarget.gameObject.GetComponent<XRUX_Base>();
        if (mainTarget.theBase != null)
        {
            width = mainTarget.theBase.transform.localScale.x;
            height = mainTarget.theBase.transform.localScale.y;
            thickness = mainTarget.theBase.transform.localScale.z;
        }
        if (mainTarget.theTitlebar != null)
        {
            height = height + mainTarget.theTitlebar.transform.localScale.y;
        }
        serializedObject.ApplyModifiedProperties();
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Main function for InspectorGUI
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public override void OnInspectorGUI()
    {
        TextMeshPro textDisplay = (mainTarget.theTitle == null) ? null : mainTarget.theTitle.GetComponent<TextMeshPro>();
        Undo.RecordObject(target, "Target changed");
        Undo.RecordObject(myTarget, "myTarget changed");
        Undo.RecordObject(textDisplay, "textDisplay changed");

        // --------------------------------------------------
        XRUX_Editor_Settings.DrawSetupHeading();
        // --------------------------------------------------

        // --------------------------------------------------
        // Set up the links to the various gameobjects
        // --------------------------------------------------
        EditorGUI.BeginChangeCheck();
        if (myTarget.mode == XRData.Mode.Advanced)
        {
            mainTarget.theBase = (GameObject) EditorGUILayout.ObjectField("Base", mainTarget.theBase, typeof(GameObject), true);
            mainTarget.minimiseButton = (GameObject) EditorGUILayout.ObjectField("Minimize Button", mainTarget.minimiseButton, typeof(GameObject), true);
            mainTarget.maximiseButton = (GameObject) EditorGUILayout.ObjectField("Maximize Button", mainTarget.maximiseButton, typeof(GameObject), true);
            mainTarget.theTitlebar = (GameObject) EditorGUILayout.ObjectField("Title Bar", mainTarget.theTitlebar, typeof(GameObject), true);
            mainTarget.theTitle = (GameObject) EditorGUILayout.ObjectField("Title Text", mainTarget.theTitle, typeof(GameObject), true);
        }

        // --------------------------------------------------
        // Change the width, height and thickness values if the gamobjects change
        // --------------------------------------------------
        if (EditorGUI.EndChangeCheck())
        {
            if (mainTarget.theBase != null)
        {
            width = mainTarget.theBase.transform.localScale.x;
            height = mainTarget.theBase.transform.localScale.y;
            thickness = mainTarget.theBase.transform.localScale.z;
        }
        if (mainTarget.theTitlebar != null)
        {
            height = height + mainTarget.theTitlebar.transform.localScale.y;
        }
            serializedObject.ApplyModifiedProperties();
        }
        
        // --------------------------------------------------
        // Get the main values for the object
        // --------------------------------------------------
        EditorGUI.BeginChangeCheck();
        width = EditorGUILayout.DelayedFloatField("Width", width);
        height = EditorGUILayout.DelayedFloatField("Height", height); 
        thickness = EditorGUILayout.DelayedFloatField("Thickness", thickness); 
        if (textDisplay != null)
        {
            textDisplay.text = EditorGUILayout.TextField("Text on titlebar", textDisplay.text);
        }

        // --------------------------------------------------
        // Set the main values for the object
        // --------------------------------------------------
        if (EditorGUI.EndChangeCheck())
        {
            mainTarget.SetSize(width, height, thickness);
        }

        // --------------------------------------------------
        // Update changes
        // --------------------------------------------------
        serializedObject.ApplyModifiedProperties();
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}