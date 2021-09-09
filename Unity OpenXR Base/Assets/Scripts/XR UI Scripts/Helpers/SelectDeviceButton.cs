/******************************************************************************************************************************************************
 * SelectDeviceButton
 * ------------------
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

public class SelectDeviceButton : MonoBehaviour
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public AudioSource clickAudio;
    public Color pressedColour = new Color(1.0f, 0.0f, 0.0f, 1.0f);

    public XRDeviceEventTypes eventToWatchFor;
    public XRDeviceActions actionToWatchFor;
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private Renderer objectToChange;
    private XRDeviceManager watcher;
    private Color originalcolor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
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

        // Find the Renderer of the gameobject of this item
        objectToChange = gameObject.GetComponent<Renderer>();

        // Save the original colour
        originalcolor = objectToChange.material.color;

        // Set up the callback
        watcher.XREventQueue.AddListener(onButtonEvent);
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
