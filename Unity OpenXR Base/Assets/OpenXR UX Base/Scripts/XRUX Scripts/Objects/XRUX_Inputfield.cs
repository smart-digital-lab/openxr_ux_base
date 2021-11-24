/**********************************************************************************************************************************************************
 * XRUX_Inputfield
 * ---------------
 *
 * 2021-08-30
 *
 * An XR UX element that collects text as if from a keyboard.
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
public interface IXRUX_Inputfield
{
    void Backspace (); // Delete the last character in the string
    void CapsLock (bool newValue); //Turn on or off capslock
    void ShiftLock (bool newValue); // Turn on or off shiftlock
    void Clear(); // Clears the text field

    void Input(string newData); // Add some text
    void Input(float newData); // Add some text
    void Input(int newData); // Add some text
    void Input(bool newData); // Add some text
    void Input(Vector3 newData); // Add some text
    void Input(XRData newData); // Add some text
    void Input(XRData newEvent, string newData); // Add some text

    void Send(); // Send the current collected text over the output
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[AddComponentMenu("OpenXR UX/Objects/XRUX InputField")]
public class XRUX_Inputfield : MonoBehaviour, IXRUX_Inputfield
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public XRData.Mode mode = XRData.Mode.User; // For use in the inspector only
    public bool singleCharacters = true;
    public UnityXRDataEvent onSend; // Functions to call when the send is called (eg when the enter key is pressed)
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private TextMeshPro textDisplay;
    private bool capsLock = false;
    private bool shiftLock = false;
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Add characters or strings to the text, taking into account the state of shiftlock and capslock
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void Input(float theItem) { Input (theItem.ToString()); }
    public void Input(int theItem) { Input (theItem.ToString()); }
    public void Input(bool theItem) { Input (theItem.ToString()); }
    public void Input(Vector3 theData) { Input(XRData.FromVector3(theData)); }
    public void Input(XRData theData) { Input(theData.ToString()); }
    public void Input(XRData theEvent, string theData) { if (theEvent.ToBool()) Input(theData); }

   
    public void Input(string theText)
    {
        if (textDisplay != null)
        {
            if (singleCharacters)
            {
                if (capsLock)
                {
                    if (shiftLock && theText.Length > 1) textDisplay.text += (theText[1].ToString()).ToUpper();
                    else textDisplay.text += (theText[0].ToString()).ToUpper();
                }
                else
                {
                    if (shiftLock)
                    {
                        if (theText.Length > 1)
                            textDisplay.text += theText[1];
                        else
                            textDisplay.text += (theText[0].ToString()).ToUpper();
                    }
                    else
                    {
                        textDisplay.text += theText[0].ToString();
                    }
                }
            }
            else
            {
                textDisplay.text += theText;
            }
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Remove the last character from the text
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void Backspace()
    {
        if (textDisplay != null)
        {
            if (textDisplay.text.Length > 0)
                textDisplay.text = textDisplay.text.Remove(textDisplay.text.Length - 1);
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Turn on or off the Capslock or Shiftlock
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void CapsLock(bool newValue)
    {
        capsLock = newValue;
    }

    public void ShiftLock(bool newValue)
    {
        shiftLock = newValue;
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
            // if (onSendString != null) onSendString.Invoke(textDisplay.text);
            if (onSend != null) onSend.Invoke(new XRData(textDisplay.text));
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set up the variables ready to go.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        textDisplay = GetComponentInChildren<TextMeshPro>();
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
