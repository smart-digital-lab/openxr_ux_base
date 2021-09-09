/******************************************************************************************************************************************************
 * ChangeColourOnEvent
 * -------------------
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

public class ChangeColourOnEvent : MonoBehaviour
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public XRDeviceEventTypes thumbstickEvent;
    public XRDeviceEventTypes triggerEvent;
    public XRDeviceEventTypes gripEvent;
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private XRDeviceManager watcher;
    private Renderer objectToChange;
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set up the variables ready for to go.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        // Find the object that has the event manager on it.  It should be the only one with tag XREvents.
        GameObject watcherGameObject = GameObject.FindWithTag("XREvents");
        if (watcherGameObject == null)
            Debug.Log("No XR Device Manager in SceneGraph that is tagged XREvents");
        else
        {
            // Get the script
            watcher = watcherGameObject.GetComponent<XRDeviceManager>();

            // Set up the callback
            watcher.XREventQueue.AddListener(onDeviceEvent);
        }

        // Find the Renderer of the gameobject of this item
        objectToChange = gameObject.GetComponent<Renderer>();
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Change the object colour depending on the events.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void onDeviceEvent(XREvent theEvent)
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
