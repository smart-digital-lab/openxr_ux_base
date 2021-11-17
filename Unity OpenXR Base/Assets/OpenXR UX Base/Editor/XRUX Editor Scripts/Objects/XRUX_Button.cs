/**********************************************************************************************************************************************************
 * XRUX_Button
 * -----------
 *
 * 2021-11-15
 *
 * Editor Layer Settings for XRUX_Button
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEditor;
using TMPro;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRUX_Button
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRUX_Button))]
public class XRUX_Button_Editor : Editor
{
    string textOnButtton;
    Vector3 colliderSize;
    // enum textPosition { InFront, Left, Right, Above, Below, User };

    public void OnEnable()
    {
        XRUX_Button myTarget = (XRUX_Button)target;
        if (myTarget.objectToResize != null)
        {
            myTarget.width = myTarget.objectToResize.transform.localScale.x;
            myTarget.height = myTarget.objectToResize.transform.localScale.y;
            myTarget.thickness = myTarget.objectToResize.transform.localScale.z;
        }        
        serializedObject.ApplyModifiedProperties();
    }
    public override void OnInspectorGUI()
    {
        XRUX_Button myTarget = (XRUX_Button)target;
        BoxCollider theCollider = myTarget.transform.GetComponent<BoxCollider>();

        myTarget.mode = (XRData.Mode) EditorGUILayout.EnumPopup("Inspector Mode", myTarget.mode);

        XRUX_Editor_Settings.DrawMainHeading("A Movable Button", "A button is a raised object that has something written or drawn on its surface, that can be interacted with using the XR controllers or mouse, and controlled by other XRUX Modules.  It can move, change color and create events when touched and activated.");

        XRUX_Editor_Settings.DrawSetupHeading();

        EditorGUI.BeginChangeCheck();
        myTarget.width = EditorGUILayout.DelayedFloatField("Width", myTarget.width);
        myTarget.height = EditorGUILayout.DelayedFloatField("Height", myTarget.height); 
        myTarget.thickness = EditorGUILayout.DelayedFloatField("Thickness", myTarget.thickness);
        if (myTarget.theTitle != null)
        {
            TextMeshPro textDisplay = myTarget.theTitle.GetComponent<TextMeshPro>();
            if (textDisplay != null) textOnButtton = textDisplay.text;
            textOnButtton = EditorGUILayout.TextField("Text on button", textOnButtton);
            myTarget.textOffset = myTarget.theTitle.transform.localPosition;
            RectTransform rt = myTarget.theTitle.GetComponent<RectTransform>();
            myTarget.textScale = rt.localScale;
    
            if (myTarget.mode == XRData.Mode.Advanced)
            {
                myTarget.affectText = EditorGUILayout.Toggle("Automate text positioning", myTarget.affectText);
                myTarget.affectCollider = EditorGUILayout.Toggle("Automate collider resizing", myTarget.affectCollider);
            }
            if (!myTarget.affectText)
            {
                myTarget.textOffset = EditorGUILayout.Vector3Field("Text offset", myTarget.textOffset);
                myTarget.textScale = EditorGUILayout.Vector3Field("Text scale", myTarget.textScale);
            }
            if ((!myTarget.affectCollider) && (theCollider != null))
            {
                colliderSize = EditorGUILayout.Vector3Field("Collider scale", theCollider.size);
            }
        }
        if (myTarget.mode == XRData.Mode.Advanced)
        {
            myTarget.theTitle = (GameObject) EditorGUILayout.ObjectField("Title text object", myTarget.theTitle, typeof(GameObject), true);
            myTarget.objectToResize = (GameObject) EditorGUILayout.ObjectField("Object to resize", myTarget.objectToResize, typeof(GameObject), true);
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (myTarget.theTitle != null)
            {
                TextMeshPro textDisplay = myTarget.theTitle.GetComponent<TextMeshPro>();
                if (textDisplay != null) textDisplay.text = textOnButtton;              
            }
            if (myTarget.objectToResize != null)
            {
                myTarget.objectToResize.transform.localScale = new Vector3(myTarget.width, myTarget.height, myTarget.thickness);
                if (theCollider != null)
                {
                    if (myTarget.affectCollider)
                    {
                        theCollider.size = myTarget.objectToResize.transform.localScale;
                    }
                    else
                    {
                        theCollider.size = colliderSize;
                    }
                }

                if (myTarget.theTitle != null)
                {
                    RectTransform rt = myTarget.theTitle.GetComponent<RectTransform>();
                    if (myTarget.affectText) 
                    {
                        myTarget.theTitle.transform.localPosition = new Vector3(myTarget.objectToResize.transform.localPosition.x, myTarget.objectToResize.transform.localPosition.y, myTarget.objectToResize.transform.localPosition.z - myTarget.thickness/1.9f);

                        if (rt != null) 
                        {
                            rt.localScale = new Vector3(myTarget.height / 10, myTarget.height / 10, 1);
                        }
                    }
                    else
                    {
                        myTarget.theTitle.transform.localPosition = myTarget.textOffset;

                        if (rt != null) 
                        {
                            rt.localScale = myTarget.textScale;
                        }
                    }
                }
            }
        }

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Title", "string | int | float | bool | Vector3 | XRData", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.LabelField("Input", "XRData", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.LabelField("Boolean XRData value to change the button state as if it was being pressed.", XRUX_Editor_Settings.helpTextStyle);

        XRUX_Editor_Settings.DrawParametersHeading();
        if (myTarget.mode == XRData.Mode.Advanced)
        {
            EditorGUILayout.LabelField("The object that will change colour when pressed.", XRUX_Editor_Settings.categoryStyle);
            myTarget.objectToColor = (Renderer) EditorGUILayout.ObjectField("Object to color", myTarget.objectToColor, typeof(Renderer), true);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("The object that will move when pressed.", XRUX_Editor_Settings.categoryStyle);
            myTarget.objectToMove = (GameObject) EditorGUILayout.ObjectField("Object to move", myTarget.objectToMove, typeof(GameObject), true);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Materials for the interaction stages.", XRUX_Editor_Settings.categoryStyle);
            myTarget.normalMaterial = (Material) EditorGUILayout.ObjectField("Normal Material", myTarget.normalMaterial, typeof(Material), true);
            myTarget.activatedMaterial = (Material) EditorGUILayout.ObjectField("Activated Material", myTarget.activatedMaterial, typeof(Material), true);
            myTarget.touchedMaterial = (Material) EditorGUILayout.ObjectField("Touched Material", myTarget.touchedMaterial, typeof(Material), true);
        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Movement axis (or none), and distance", XRUX_Editor_Settings.categoryStyle);
        myTarget.movementAxis = (XRUX_Button.XRGenericButtonAxis) EditorGUILayout.EnumPopup("Movement Axis", myTarget.movementAxis);
        if (myTarget.movementAxis != XRUX_Button.XRGenericButtonAxis.None)
        {
            myTarget.movementAmount = EditorGUILayout.FloatField("Movement Amount", myTarget.movementAmount);
        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Toggle or Momentary Movement Style", XRUX_Editor_Settings.categoryStyle);
        myTarget.movementStyle = (XRUX_Button.XRGenericButtonMovement) EditorGUILayout.EnumPopup("Movement Style", myTarget.movementStyle);
        EditorGUILayout.LabelField("Toggle buttons turn on or off once for each press, whereas Momentary buttons click on and off with just one press.", XRUX_Editor_Settings.helpTextStyle);
        EditorGUILayout.Space();
        XRUX_Editor_Settings.DrawOutputsHeading();
        var prop = serializedObject.FindProperty("onChange"); EditorGUILayout.PropertyField(prop, true);    
        if (myTarget.mode == XRData.Mode.Advanced)
        {
            var prop2 = serializedObject.FindProperty("onClick"); EditorGUILayout.PropertyField(prop2, true);    
            var prop3 = serializedObject.FindProperty("onUnclick"); EditorGUILayout.PropertyField(prop3, true);    
            var prop4 = serializedObject.FindProperty("onTouch"); EditorGUILayout.PropertyField(prop4, true);   
            var prop5 = serializedObject.FindProperty("onUntouch"); EditorGUILayout.PropertyField(prop5, true); 
        }  
        EditorGUILayout.Space();
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
