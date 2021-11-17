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
using TMPro;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// XRUX_ButtonGroup
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[CustomEditor(typeof(XRUX_ButtonGroup))]
public class XRUX_ButtonGroup_Editor : Editor
{
    int prevDynamicButtonsLength = 0;

    public override void OnInspectorGUI()
    {
        XRUX_ButtonGroup myTarget = (XRUX_ButtonGroup)target;

        XRUX_Editor_Settings.DrawMainHeading("A Group of Buttons", "A group of buttons where one can be pressed at a time.  Set the prefab to use, then add as many buttons as required by entering their titles.");

        XRUX_Editor_Settings.DrawSetupHeading();
        // EditorGUILayout.LabelField("Dynamic Buttons, created from the prefab.", XRUX_Editor_Settings.categoryStyle);
        myTarget.buttonPrefab = (GameObject) EditorGUILayout.ObjectField("Button Prefab", myTarget.buttonPrefab, typeof(GameObject), true);
        if (myTarget.buttonPrefab != null)
        {
            EditorGUI.BeginChangeCheck();
            myTarget.dynamicButtonSpacing = EditorGUILayout.FloatField("Dynamic Button Spacing", myTarget.dynamicButtonSpacing);
            EditorGUILayout.LabelField("Titles of Buttons to be created dynamically.", XRUX_Editor_Settings.helpTextStyle);
            var prop = serializedObject.FindProperty("dynamicButtons"); EditorGUILayout.PropertyField(prop, true);
            if ((myTarget.dynamicButtons.Length != prevDynamicButtonsLength) || EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                CreateDynamicButtons(myTarget);
                prevDynamicButtonsLength = myTarget.dynamicButtons.Length;
            }
        }

        XRUX_Editor_Settings.DrawInputsHeading();
        EditorGUILayout.LabelField("Input", "XRData", XRUX_Editor_Settings.fieldStyle);
        EditorGUILayout.LabelField("Integer value to change which button is selected as if it was being pressed.", XRUX_Editor_Settings.helpTextStyle);

        XRUX_Editor_Settings.DrawParametersHeading(); 

        XRUX_Editor_Settings.DrawOutputsHeading();
        var prop2 = serializedObject.FindProperty("onChange"); EditorGUILayout.PropertyField(prop2, true);    
 
        EditorGUILayout.Space();
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed) EditorUtility.SetDirty(target);
    }


    private void CreateDynamicButtons(XRUX_ButtonGroup myTarget)
    {
        // Create the dynamic buttons (if any).

        if (myTarget.buttonPrefab == null)
        {
            Debug.Log("No Button Prefab to duplicate");
        }
        else
        {
            // Remove the existing buttons
            XRUX_Button[] allButtons = myTarget.transform.GetComponentsInChildren<XRUX_Button>();
            foreach (XRUX_Button buttonScript in allButtons)
            {
                DestroyImmediate(buttonScript.gameObject, true);
            }
                        
            int counter = 0;
            foreach (string title in myTarget.dynamicButtons)
            {
                // Create a new Button
                GameObject newButton = Instantiate(myTarget.buttonPrefab);
                // Get the transform to use as the grouping object
                Transform group = myTarget.gameObject.transform;
                // Add the new button to the group
                newButton.transform.SetParent(group);
                // Position it relative to the zero position (going down from the top)
                newButton.transform.localPosition = new Vector3(0, (myTarget.fixedItems + counter) * -myTarget.dynamicButtonSpacing, 0);
                newButton.transform.localRotation = new Quaternion(0, 0, 0, 1);

                XRUX_Button buttonScript = newButton.GetComponentInChildren<XRUX_Button>();
                if (buttonScript != null)
                {
                    XRUX_SetText textToChange = buttonScript.GetComponentInChildren<XRUX_SetText>();
                    if (textToChange != null)
                    {
                        TextMeshPro textDisplay = textToChange.GetComponent<TextMeshPro>();
                        if (textDisplay != null)
                        {
                            textDisplay.text = title;
                        }
                    }
                }
                counter++;
            }
        }
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
