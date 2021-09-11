/******************************************************************************************************************************************************
 * MakeVisibleOnEvent
 * ------------------
 *
 * 2021-08-25
 * 
 * Reacts to events and makes the gameobject the script is sitting on active or inactive.  This has the effect of making it visible or
 * invisible.
 * 
 * Roy Davies, Smart Digital Lab, University of Auckland.
 ******************************************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeVisibleOnEvent : MonoBehaviour
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public XRDeviceEventTypes eventToWatchFor;
    public XRDeviceActions actionToWatchFor;
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private XRDeviceManager watcher;
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
            watcher = watcherGameObject.GetComponent<XRDeviceManager>();

        // Set up the callback
        watcher.XREventQueue.AddListener(onButtonEvent);

        // Start with the object being inactive / invisible
        gameObject.SetActive(false);
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Make the object visible or invisible.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void onButtonEvent(XREvent theEvent)
    {
        if ((theEvent.eventType == eventToWatchFor) && (theEvent.eventAction == actionToWatchFor))
        {
            gameObject.SetActive(theEvent.eventBool);
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
