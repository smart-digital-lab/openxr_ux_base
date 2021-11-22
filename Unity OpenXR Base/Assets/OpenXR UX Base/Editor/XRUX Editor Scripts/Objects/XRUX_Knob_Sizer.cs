/**********************************************************************************************************************************************************
 * XRUX_Knob_Sizer
 * ---------------
 *
 * 2021-11-20
 *
 * Editor Layer Settings for XRUX_Knob_Sizer
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;
using TMPro;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRUX_Knob_Sizer
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRUX_Knob_Sizer))]
public class XRUX_Knob_Sizer_Editor : Editor
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    float width, height, thickness;      // The dimensions of the object to be used by the editor
    XRUX_Knob_Sizer mainTarget;
    XRUX_Knob myTarget;
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set up certain values when enabled
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void OnEnable()
    {
        mainTarget = (XRUX_Knob_Sizer)target;
        myTarget = (XRUX_Knob)mainTarget.gameObject.GetComponent<XRUX_Knob>();
        if (mainTarget.objectToResize != null)
        {
            width = mainTarget.objectToResize.transform.localScale.x;
            height = mainTarget.objectToResize.transform.localScale.y;
            thickness = mainTarget.objectToResize.transform.localScale.z;
        }        
        serializedObject.ApplyModifiedProperties();
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Main function for InspectorGUI
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public override void OnInspectorGUI()
    {
        TextMeshPro textDisplay = (mainTarget.titleObject == null) ? null : mainTarget.titleObject.GetComponent<TextMeshPro>();
        Undo.RecordObject(target, "Target changed");
        Undo.RecordObject(myTarget, "myTarget changed");
        Undo.RecordObject(textDisplay, "textDisplay changed");
        
        // --------------------------------------------------
        XRUX_Editor_Settings.DrawSetupHeading();
        // --------------------------------------------------

        // --------------------------------------------------
        // Button size and position
        // --------------------------------------------------
        width = EditorGUILayout.DelayedFloatField("Width", width);
        height = EditorGUILayout.DelayedFloatField("Height", height); 
        thickness = EditorGUILayout.DelayedFloatField("Thickness", thickness);
        // --------------------------------------------------
        // Inputs related to the title and collider
        // --------------------------------------------------
        if (textDisplay != null)
        {
            textDisplay.text = EditorGUILayout.TextField("Text on knob", textDisplay.text);
        }

        // --------------------------------------------------
        // Button Title and resizer objects
        // --------------------------------------------------
        if (myTarget.mode == XRData.Mode.Advanced)
        {
            mainTarget.titleObject = (GameObject) EditorGUILayout.ObjectField("Title text object", mainTarget.titleObject, typeof(GameObject), true);
            mainTarget.objectToResize = (GameObject) EditorGUILayout.ObjectField("Object to resize", mainTarget.objectToResize, typeof(GameObject), true);
        }

        // --------------------------------------------------
        // Set size
        // --------------------------------------------------
        mainTarget.SetSize(width, height, thickness);

        // --------------------------------------------------
        // Update changes
        // --------------------------------------------------
        serializedObject.ApplyModifiedProperties();
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}