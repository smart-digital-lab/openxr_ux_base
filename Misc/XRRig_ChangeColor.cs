/******************************************************************************************************************************************************
 * XRRig_ChangeColor
 * -----------------
 *
 * 2021-08-29
 *
 * Changes the colour and transparency of the object the script is on depending on the VR Device Trigger, Grip and Thumbstick as set in
 * the inspector.  The material has to have transparency for this to work.
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 ******************************************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Public functions
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public interface _XRRig_ChangeColor
{
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public class XRRig_ChangeColor : MonoBehaviour, _XRRig_ChangeColor
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    [Header("____________________________________________________________________________________________________")]
    [Header("Change colour on a controller button for a given event.\n____________________________________________________________________________________________________")]
    [Header("INPUTS\n\n - Events from XR Controllers.")]

    [Header("____________________________________________________________________________________________________")]
    [Header("SETTINGS")]
    [Header("The Controller thumbstick event to react to")]
    public XRDeviceEventTypes thumbstickEvent;
    [Header("The Controller trigger event to react to")]
    public XRDeviceEventTypes triggerEvent;
    [Header("The Controller grip event to react to")]
    public XRDeviceEventTypes gripEvent;
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private Renderer objectToChange;
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set up the variables ready for to go.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        // Listen for events coming from the XR Controllers and other devices
        if (XRRig.EventQueue != null) XRRig.EventQueue.AddListener(onDeviceEvent);

        // Find the Renderer of the gameobject of this item
        objectToChange = gameObject.GetComponent<Renderer>();
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Change the object colour depending on the events.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void onDeviceEvent(XREvent theEvent)
    {
        if ((theEvent.eventType == thumbstickEvent) && (theEvent.eventAction == XRDeviceActions.MOVE))
        {
            Color pressedColour = new Color((theEvent.eventVector.x + 1.0f) / 2.0f, (theEvent.eventVector.y + 1.0f) / 2.0f, objectToChange.material.color.b, objectToChange.material.color.a);
            objectToChange.material.color = pressedColour;
        }
        else if ((theEvent.eventType == triggerEvent) && (theEvent.eventAction == XRDeviceActions.MOVE))
        {
            Color pressedColour = new Color(objectToChange.material.color.r, objectToChange.material.color.g, theEvent.eventFloat, objectToChange.material.color.a);
            objectToChange.material.color = pressedColour;
        }
        else if ((theEvent.eventType == gripEvent) && (theEvent.eventAction == XRDeviceActions.MOVE))
        {
            Color pressedColour = new Color(objectToChange.material.color.r, objectToChange.material.color.g, objectToChange.material.color.b, theEvent.eventFloat);
            objectToChange.material.color = pressedColour;
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
