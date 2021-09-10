/**********************************************************************************************************************************************************
 * XRTextfield
 * ------------
 *
 * 2021-08-30
 *
 * An XR UX element to show text.
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Public functions
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public interface _XRTextfield
{
    void Clear(); // Clears the text field

    void Input(string newTitle); // Add some text
    void Input(float newTitle); // Add some text
    void Input(int newTitle); // Add some text
    void Input(bool newTitle); // Add some text
    void Input(XRData newData); // Add some text

    void Send(); // Send the current collected text over the output
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[AddComponentMenu("XR/UX/Objects/XRTextfield")]
public class XRTextfield : MonoBehaviour, _XRTextfield
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    [Header("____________________________________________________________________________________________________")]
    [Header("A simple one-line text field.\n____________________________________________________________________________________________________")]
    [Header("INPUTS\n\n - Input( [ int | float | bool | string | XRData ] ) - Input data to display.\n" + 
            " - Send() - Sends the currently collected text on the onSend Event queue.")]

    [Header("____________________________________________________________________________________________________")]
    [Header("SETTINGS")]

    [Header("____________________________________________________________________________________________________")]
    [Header("OUTPUTS")]
    public UnityXRDataEvent onSend; // Functions to call when the send is called
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private TextMeshPro textDisplay;
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Change the text
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void Input(float theItem) { Input (theItem.ToString()); }
    public void Input(int theItem) { Input (theItem.ToString()); }
    public void Input(bool theItem) { Input (theItem.ToString()); }
    public void Input(XRData theData) { Input (theData.ToString()); }

    public void Input(string theText)
    {
        if (textDisplay != null)
        {
            textDisplay.text = theText;
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Clear the text
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void Clear()
    {
        if (textDisplay != null)
        {
            textDisplay.text = "";
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void Send()
    {
        if (textDisplay != null)
        {
            if (onSend != null) onSend.Invoke(new XRData(textDisplay.text));
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set up the variables ready to go.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Awake()
    {
        textDisplay = GetComponentInChildren<TextMeshPro>();
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
