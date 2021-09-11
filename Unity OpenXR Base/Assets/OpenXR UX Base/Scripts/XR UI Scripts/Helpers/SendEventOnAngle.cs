/**********************************************************************************************************************************************************
 * SendEventOnAngle
 * ----------------
 *
 * 2021-08-25
 * 
 * Checks the angle between a source object and a destination object and sends an event if the source is 'looking at' the target.
 * 
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { FORWARD, BACK }

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Public functions
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public interface _SendEventOnAngle
{
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public class SendEventOnAngle : MonoBehaviour, _SendEventOnAngle
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public Transform source; // Source object to look from
    public Transform target; // Target object to look towards
    public float triggerAngle = 20.0f; // The angle at which to trigger the event
    public Direction zDirection = Direction.FORWARD; // Are we looking along the Z axis or back along the Z axis?
    public XRDeviceEventTypes eventToSendOnTrigger; // The event to trigger
    public XRDeviceActions actionToSendOnTrigger; // The action to trigger
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private XRDeviceManager watcher; // The XR Event Manager
    private bool firstTime = true; // First time running?
    private bool previousLookingAt = false; // Used to make sure we only send an event when something changes.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set up the variables ready to go.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        // Find the object that has the event manager on it.  It should be the only one with tag XREvents.
        GameObject watcherGameObject = GameObject.FindWithTag("XREvents");
        if (watcherGameObject == null)
            Debug.Log("No XR Device Manager in SceneGraph that is tagged XREvents");
        else
            watcher = watcherGameObject.GetComponent<XRDeviceManager>();
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Check the angle on each frame, and if there is a change in circumstance, send an event
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        // Get the Vector between the source and target
        Vector3 targetDir = source.position - target.position;
        // Determine the angle between the forward or back direction and the above vector.
        float angle = Vector3.Angle(targetDir, (zDirection == Direction.FORWARD)? -source.forward : source.forward);
        // If the angle is less than the trigger angle, then - yup, we're looking at it.
        bool lookingAt = (angle < triggerAngle);

        // Create and send an event if something has changed.
        if ((previousLookingAt != lookingAt) || firstTime)
        {
            // Create an event to send
            XREvent eventToSend = new XREvent();
            eventToSend.eventType = eventToSendOnTrigger;
            eventToSend.eventAction = actionToSendOnTrigger;
            eventToSend.eventBool = lookingAt;
            watcher.XREventQueue.Invoke(eventToSend);

            previousLookingAt = lookingAt;
            firstTime = false;
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}

