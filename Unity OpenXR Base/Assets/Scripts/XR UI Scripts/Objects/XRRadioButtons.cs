/**********************************************************************************************************************************************************
 * XRRadioButtons
 * --------------
 *
 * 2021-08-30
 *
 * A set of radio buttons where one or more (or none) can be pressed at a time.  In this code, you can either specify the radio buttons by hand in Unity by
 * creating radiobuttons and adding them to the radiobutton group, or you can just fill in the Titles and have a list of buttons created dynamically at
 * runtime.  Or both.
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Public functions
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public interface _XRRadioButtons
{
    void Input(XRData newData);
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[AddComponentMenu("OpenXR UX/Objects/XRRadioButtons")]
public class XRRadioButtons : MonoBehaviour, _XRRadioButtons
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    [Header("____________________________________________________________________________________________________")]
    [Header("The Radio Buttons Group\n\nAdd Radio Buttons as children or create them dynamically below\n____________________________________________________________________________________________________")]
    [Header("INPUTS\n\n - Input( XRData ) - Integer value to change which button is selected as if it was being pressed.")]

    [Header("____________________________________________________________________________________________________")]
    [Header("SETTINGS")]
    [Header("Prefab to be used when creating dynamic Radio Buttons.")]
    public GameObject radioButtonPrefab;    // Link to the radio button prefab
    [Header("Titles of Radio Buttons to be created dynamically.\nThese will appear below the last Radio Button child object.")]
    public string[] dynamicRadioButtons;    // Radio buttons that are created dynamically with the titles filled in from here.
    [Header("Vertical spacing between radio buttons.")]
    public float dynamicRadioButtonSpacing = 0.025f;
    [Header("One or many buttons clicked at once.")]
    public bool singleButtonOnly = true;

    [Header("____________________________________________________________________________________________________")]
    [Header("OUTPUTS")]
    public UnityXRDataEvent onChange; // Both the number and the string are sent.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variales
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private List<XRRadioButton> allRadioButtons; // The radio buttons that are children of this GameObject folowed by the radio buttons created dynamically.
    private bool firstTime = true;
    private int fixedItems = 0; // The number of fixed Items (ie the ones that are children of this GameObject)
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Change the state of the given button
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void Input(XRData newData)
    {
        RadioButtonToggleFromInput(newData.ToInt(), newData.quietly);       
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Start is called before the first frame update
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        // Create the list
        allRadioButtons = new List<XRRadioButton>();

        // Find all the radio buttons in the children of this GameObject - these are the fixed items
        XRRadioButton[] existingRadioButtons = GetComponentsInChildren<XRRadioButton>();
        foreach (XRRadioButton radioButtonScript in existingRadioButtons)
        {
            // Add this radio button to the list
            allRadioButtons.Add(radioButtonScript);
        }

        // Store the number of fixed items in the list after adding the fixed items to the list.
        fixedItems = allRadioButtons.Count;

        // Create the dynamic radio buttons.
        if (radioButtonPrefab == null)
        {
            Debug.Log("No RadioButton Prefab to duplicate");
        }
        else
        {
            int counter = 0;
            foreach (string title in dynamicRadioButtons)
            {
                // Create a new RadioButton
                GameObject newRadioButton = Instantiate(radioButtonPrefab);
                // Get the transform to use as the grouping object
                Transform group = this.gameObject.transform;
                // Add the new radio button to the group
                newRadioButton.transform.SetParent(group);
                // Position it relative to the zero position (going down from the top)
                newRadioButton.transform.localPosition = new Vector3(0, (fixedItems + counter) * -dynamicRadioButtonSpacing, 0);
                newRadioButton.transform.localRotation = new Quaternion(0, 0, 0, 1);
                // Set the title
                XRRadioButton radioButtonScript = newRadioButton.GetComponent<XRRadioButton>();
                // Add it to the list of buttons already there.
                allRadioButtons.Add(radioButtonScript);
                counter++;
            }
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set things up
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        // Run this once after all the start functions in the other GameObjects have been run so that all the links etc are in the right places before
        // we set up the links to the listeners etc, text in titles etc.  Some GameObjects need to do it this way.
        if (firstTime)
        {
            // Set all the buttons off
            for (int counter = 0; counter < allRadioButtons.Count; counter++)
            {
                allRadioButtons[counter].Input(new XRData(false, true));
                int temp = counter; // Assign this to a temporary variable so the lambda functions below works wtih the correct parameter value
                allRadioButtons[counter].onRadioOn.AddListener(() => { RadioButtonToggleOn(temp); });
                allRadioButtons[counter].onRadioOff.AddListener(() => { RadioButtonToggleOff(temp); });

                // Set the titles of the dynamic items
                if (counter >= fixedItems)
                    allRadioButtons[counter].Title(dynamicRadioButtons[counter - fixedItems]);
            }
            firstTime = false;
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Turn all the other buttons off
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void RadioButtonToggleFromInput(int buttonNumber, bool quietly = false)
    {
        // Turn on or off the designated radio button from an Input Command (ie from some other XR entity)
        allRadioButtons[buttonNumber % allRadioButtons.Count].Input(new XRData(!allRadioButtons[buttonNumber % allRadioButtons.Count].ButtonState(), quietly));
    }
    private void RadioButtonToggleOff(int buttonNumber, bool quietly = false)
    {
        // Send Events
        if ((onChange != null) && !quietly) onChange.Invoke(new XRData(-1));
    }
    private void RadioButtonToggleOn(int buttonNumber, bool quietly = false)
    {
        // Turn off the others
        if (singleButtonOnly)
        {
            for (int counter = 0; counter < allRadioButtons.Count; counter++)
            {
                if (counter != buttonNumber)
                {
                    allRadioButtons[counter].Input(new XRData(false, true));
                }
            }
        }

        // Send Events
        if ((onChange != null) && !quietly) onChange.Invoke(new XRData(buttonNumber));
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
