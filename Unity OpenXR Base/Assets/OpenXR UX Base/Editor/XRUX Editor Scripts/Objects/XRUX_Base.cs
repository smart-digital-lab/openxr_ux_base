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
    float x, y;
    string textOnTitlebar;

    void OnEnable()
    {
        XRUX_Base myTarget = (XRUX_Base)target;
        if (myTarget.theBase != null)
        {
            x = myTarget.theBase.transform.localScale.x;
            y = myTarget.theBase.transform.localScale.y;
        }
        if (myTarget.theTitlebar != null)
        {
            y = y + myTarget.theTitlebar.transform.localScale.y;
        }
        serializedObject.ApplyModifiedProperties();
    }
    public override void OnInspectorGUI()
    {
        XRUX_Base myTarget = (XRUX_Base)target;
        myTarget.mode = (XRData.Mode) EditorGUILayout.EnumPopup("Inspector Mode", myTarget.mode);

        XRUX_Editor_Settings.DrawMainHeading("Minimisable surface for XRUX Objects", "A flat surface to put XR Objects on that can be minimised and maximised.  Change the shape of the base object for non-flat UXs.  Primarily used by the XRUX_Base prefab.");

        XRUX_Editor_Settings.DrawSetupHeading();

        EditorGUI.BeginChangeCheck();
        if (myTarget.mode == XRData.Mode.Advanced)
        {
            myTarget.theBase = (GameObject) EditorGUILayout.ObjectField("Base", myTarget.theBase, typeof(GameObject), true);
            myTarget.theButton = (GameObject) EditorGUILayout.ObjectField("Minimize Button", myTarget.theButton, typeof(GameObject), true);
            myTarget.theTitlebar = (GameObject) EditorGUILayout.ObjectField("Title Bar", myTarget.theTitlebar, typeof(GameObject), true);
            myTarget.theTitle = (GameObject) EditorGUILayout.ObjectField("Title Text", myTarget.theTitle, typeof(GameObject), true);
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (myTarget.theBase != null)
            {
                x = myTarget.theBase.transform.localScale.x;
                y = myTarget.theBase.transform.localScale.y;
            }
            if (myTarget.theTitlebar != null)
            {
                y = y + myTarget.theTitlebar.transform.localScale.y;
            }
            serializedObject.ApplyModifiedProperties();
        }
        
        EditorGUI.BeginChangeCheck();
        x = EditorGUILayout.DelayedFloatField("Width", x);
        y = EditorGUILayout.DelayedFloatField("Height", y); 
        if (myTarget.theTitle != null)
        {
            TextMeshPro textDisplay = myTarget.theTitle.GetComponent<TextMeshPro>();
            if (textDisplay != null) textOnTitlebar = textDisplay.text;
            textOnTitlebar = EditorGUILayout.TextField("Text on titlebar", textOnTitlebar);
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (myTarget.theBase != null)
            {
                if (myTarget.theTitle != null)
                {
                    TextMeshPro textDisplay = myTarget.theTitle.GetComponent<TextMeshPro>();
                    if (textDisplay != null) textDisplay.text = textOnTitlebar;              
                }
                if (myTarget.theTitlebar != null)
                {
                    myTarget.theBase.transform.localScale = new Vector3(x, y - myTarget.theTitlebar.transform.localScale.y, myTarget.theBase.transform.localScale.z);
                    myTarget.theBase.transform.localPosition = new Vector3(0, 0, 0);
                    if (myTarget.theButton != null)
                    {
                        XRUX_Button buttonScript = myTarget.theButton.GetComponent<XRUX_Button>();
                        if (buttonScript != null)
                        {
                            myTarget.theTitlebar.transform.localScale = new Vector3(x - buttonScript.width, myTarget.theTitlebar.transform.localScale.y, myTarget.theTitlebar.transform.localScale.z);
                            myTarget.theTitlebar.transform.localPosition = new Vector3(myTarget.theBase.transform.localPosition.x - buttonScript.width/2, y/2, myTarget.theBase.transform.localPosition.z);

                            if (myTarget.theTitle != null)
                            {
                                RectTransform rt = myTarget.theTitle.GetComponent<RectTransform>();
                                if (rt != null) 
                                {
                                    rt.sizeDelta = new Vector2(myTarget.theTitlebar.transform.localScale.x / rt.localScale.x, 1);
                                }
                                myTarget.theTitle.transform.localPosition = new Vector3(myTarget.theBase.transform.localPosition.x - buttonScript.width/2, y/2 - myTarget.theTitlebar.transform.localScale.y/2 + 0.001f, myTarget.theBase.transform.localPosition.z - myTarget.theTitlebar.transform.localScale.z / 1.9f);
                            }
                        }
                        myTarget.theButton.transform.localPosition = new Vector3(x/2 - buttonScript.width/2, y/2, myTarget.theTitlebar.transform.localPosition.z);
                    }
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        // myTarget.mode = (XRData.Mode) EditorGUILayout.EnumPopup("Inspector Mode", myTarget.mode);
      
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
