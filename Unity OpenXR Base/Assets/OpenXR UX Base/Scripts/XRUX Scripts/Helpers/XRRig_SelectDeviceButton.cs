/******************************************************************************************************************************************************
 * XRRig_SelectDeviceButton
 * ------------------------
 *
 * 2021-08-29
 * 
 * Reacts to button events on the controllers and can be used to change the colour and makes a sound when the button goes on or off.
 * Used primarily on the controllers to show when buttons are touched and pressed, but could be useful elsewhere.
 * 
 * Roy Davies, Smart Digital Lab, University of Auckland.
 ******************************************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Public functions
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public interface IXRRig_SelectDeviceButton
{
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public class XRRig_SelectDeviceButton : MonoBehaviour, IXRRig_SelectDeviceButton
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    [Header("____________________________________________________________________________________________________")]
    [Header("Change colour and play a sound on given event and action.\n____________________________________________________________________________________________________")]
    [Header("INPUTS")]
    [Header("Event to watch for")]
    public XRDeviceEventTypes eventToWatchFor;
    [Header("Action to watch for")]
    public XRDeviceActions actionToWatchFor;
    [Header("SETTINGS")]
    [Header("Sound to play (eg a click) or None if no sound to be played")]
    public AudioSource clickAudio;
    [Header("Color to change to")]
    public Color pressedColour = new Color(1.0f, 0.0f, 0.0f, 1.0f);
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private Renderer objectToChange;
    private Color originalcolor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set up the variables ready for to go.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        // Listen for events coming from the XR Controllers and other devices
        if (XRRig.EventQueue != null) XRRig.EventQueue.AddListener(onButtonEvent);
        
        // Find the Renderer of the gameobject of this item
        objectToChange = gameObject.GetComponent<Renderer>();

        // Save the original colour
        if (objectToChange != null) originalcolor = objectToChange.material.color;
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Play the noise and change the colour
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void onButtonEvent(XREvent theEvent)
    {
        if ((theEvent.eventType == eventToWatchFor) && (theEvent.eventAction == actionToWatchFor))
        {
            if (theEvent.eventBool)
            {
                if (clickAudio != null) clickAudio.Play();
                objectToChange.material.color = pressedColour;
            }
            else
            {
                if (clickAudio != null) clickAudio.Play();
                objectToChange.material.color = originalcolor;
            }
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
