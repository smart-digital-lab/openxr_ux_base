/**********************************************************************************************************************************************************
 * XRRig_Manager
 * -------------
 *
 * 2021-08-25
 *
 * Monitor the OpenXR device values and sends these out via Unity Events to be picked up by other objects that need to know.
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// The events that are sent around when buttons are pressed or thumbsticks moved and when we want things to happen.
// These are sent as unity events which can be picked up by any object in the scenegraph.
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public enum XRDeviceEventTypes {
    left_trigger,
    left_grip,
    left_primary,
    left_secondary,
    left_thumbstick,
    left_pointer,
    left_UI,
    right_trigger,
    right_grip,
    right_primary,
    right_secondary,
    right_thumbstick,
    right_pointer,
    right_UI,
    heads_down_UI,
    heads_up_UI,
    menu_button,
    scene,
    console,
    mouse_scroll,
    _
};
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Action modifiers for the Events above.
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public enum XRDeviceActions
{
    TOUCH, CLICK, MOVE, LOOKAT, POINTAT, CHANGE, UP, DOWN, _
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// The type of data that can be sent via the XR Event
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[Serializable]
public class XREvent
{
    public XRDeviceEventTypes eventType;
    public XRDeviceActions eventAction;
    public bool eventBool;      // For buttons
    public float eventFloat;    // For single values (eg the trigger)
    public Vector2 eventVector; // For double values (eg the joystick)
    public XRData data;         // Misc Data
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// The unity event queue for XR events
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[Serializable]
public class XRDeviceEvents : UnityEvent<XREvent> { };
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Public functions
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public interface IXRRig_Manager
{
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public class XRRig_Manager : MonoBehaviour, IXRRig_Manager
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public XRDeviceEvents XREventQueue; // The event queue that all the XR UX elements will need to look at to get events.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private List<InputDevice> leftHandDevices;
    private List<InputDevice> rightHandDevices;

    // These arrays are used to make sure we only send events when something changes, rather than every frame.
    private bool[,] storedEvents = new bool[(int)XRDeviceEventTypes._, (int)XRDeviceActions._];
    private float[] storedValues = new float[(int)XRDeviceEventTypes._];
    private Vector2[] storedVectors = new Vector2[(int)XRDeviceEventTypes._];
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set up important variables and data structures
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Awake()
    {
        leftHandDevices = new List<InputDevice>();
        rightHandDevices = new List<InputDevice>();
        XREventQueue = new XRDeviceEvents();

        // Get the devices in the left and right hands respectively
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftHandDevices);
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);
        Application.targetFrameRate = 90;
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------


    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Test for devices - sometimes they can take a while to become available
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void TestForDevices()
    {
        if (leftHandDevices != null)
        {
            if (leftHandDevices.Count == 0)
            {
                InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftHandDevices);
            }
        }
        if (rightHandDevices != null)
        {
            if (rightHandDevices.Count == 0)
            {
                InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);
            }
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Test whether the given button on the give device has activated, and if so send a message.
    // Also send a message if the button is released.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void TestButton(List<InputDevice> XRdevices, InputFeatureUsage<bool> usage, XRDeviceEventTypes eventToTrigger, XRDeviceActions eventAction)
    {
        foreach (var device in XRdevices)
        {
            bool buttonState = false;
            if (device.TryGetFeatureValue(usage, out buttonState))
            {
                if (storedEvents[(int)eventToTrigger, (int)eventAction] != buttonState)
                {
                    storedEvents[(int)eventToTrigger, (int)eventAction] = buttonState;

                    XREvent eventToSend = new XREvent();
                    eventToSend.eventType = eventToTrigger;
                    eventToSend.eventBool = buttonState;
                    eventToSend.eventAction = eventAction;

                    XREventQueue.Invoke(eventToSend);
                }
            }
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Test whether the thumbstick has moved and if so, send the x,y values as an event
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void TestThumbStick(List<InputDevice> XRdevices, InputFeatureUsage<Vector2> usage, XRDeviceEventTypes eventToTrigger)
    {
        foreach (var device in XRdevices)
        {
            Vector2 twoDAxis = new Vector2(0, 0);
            if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out twoDAxis))
            {
                if (storedVectors[(int)eventToTrigger] != twoDAxis)
                {
                    storedVectors[(int)eventToTrigger] = twoDAxis;

                    XREvent eventToSend = new XREvent();
                    eventToSend.eventType = eventToTrigger;
                    eventToSend.eventVector = twoDAxis;
                    eventToSend.eventAction = XRDeviceActions.MOVE;

                    XREventQueue.Invoke(eventToSend);
                }
            }
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Test whether a variable button (such as the trigger and grip) have moved and if so, send the value as an event
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void TestValue(List<InputDevice> XRdevices, InputFeatureUsage<float> usage, XRDeviceEventTypes eventToTrigger)
    {
        foreach (var device in XRdevices)
        {
            float singleValue = 0.0f;
            if (device.TryGetFeatureValue(usage, out singleValue))
            {
                // Send the actual value
                if (storedValues[(int)eventToTrigger] != singleValue)
                {
                    storedValues[(int)eventToTrigger] = singleValue;

                    XREvent eventToSend = new XREvent();
                    eventToSend.eventType = eventToTrigger;
                    eventToSend.eventFloat = singleValue;
                    eventToSend.eventAction = XRDeviceActions.MOVE;

                    XREventQueue.Invoke(eventToSend);
                }

                // Send a touch event if the button is moved slightly (or likewise, released)
                bool buttonStateTouch = (singleValue >= 0.001); //&& (singleValue < 0.5);
                if (storedEvents[(int)eventToTrigger, (int)XRDeviceActions.TOUCH] != buttonStateTouch)
                {
                    storedEvents[(int)eventToTrigger, (int)XRDeviceActions.TOUCH] = buttonStateTouch;

                    XREvent eventToSend = new XREvent();
                    eventToSend.eventType = eventToTrigger;
                    eventToSend.eventBool = buttonStateTouch;
                    eventToSend.eventAction = XRDeviceActions.TOUCH;

                    XREventQueue.Invoke(eventToSend);
                }
                else
                {
                    // Send a click event.  We do it this way rather than using the inbuilt click event as the inbuilt one is too sensitive.
                    // This will occur only once the touch state event has been sent (above)
                    bool buttonStateClick = (singleValue >= 0.5);
                    if (storedEvents[(int)eventToTrigger, (int)XRDeviceActions.CLICK] != buttonStateClick)
                    {
                        storedEvents[(int)eventToTrigger, (int)XRDeviceActions.CLICK] = buttonStateClick;

                        XREvent eventToSend = new XREvent();
                        eventToSend.eventType = eventToTrigger;
                        eventToSend.eventBool = buttonStateClick;
                        eventToSend.eventAction = XRDeviceActions.CLICK;

                        XREventQueue.Invoke(eventToSend);
                    }
                }
            }
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    private void SendVectorEvent(XRDeviceEventTypes eventToTrigger, Vector2 twoDAxis)
    {
        XREvent eventToSend = new XREvent();
        eventToSend.eventType = eventToTrigger;
        eventToSend.eventVector = twoDAxis;
        eventToSend.eventAction = XRDeviceActions.MOVE;

        XREventQueue.Invoke(eventToSend);
    }
    private void SendKeyEvent(XRDeviceEventTypes eventToTrigger, XRDeviceActions eventAction, bool buttonState)
    {
        XREvent eventToSend = new XREvent();
        eventToSend.eventType = eventToTrigger;
        eventToSend.eventBool = buttonState;
        eventToSend.eventAction = eventAction;

        XREventQueue.Invoke(eventToSend);
    }
    private void SendMouseEvent(XRDeviceEventTypes eventToTrigger, XRDeviceActions eventAction)
    {
        XREvent eventToSend = new XREvent();
        eventToSend.eventType = eventToTrigger;
        eventToSend.eventAction = eventAction;

        XREventQueue.Invoke(eventToSend);
    }



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Each frame, check for any new actions that need sending out.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        TestForDevices();

        if ((leftHandDevices.Count > 0) || (rightHandDevices.Count > 0))
        {
            // Check each of the buttons on the left and right controllers and send the appropriate event depending on what is happening.
            //TestButton(leftHandDevices, CommonUsages.triggerButton,         XRDeviceEventTypes.left_trigger,        XRDeviceActions.CLICK);
            TestButton(leftHandDevices, CommonUsages.primaryButton,         XRDeviceEventTypes.left_primary,        XRDeviceActions.CLICK);
            TestButton(leftHandDevices, CommonUsages.secondaryButton,       XRDeviceEventTypes.left_secondary,      XRDeviceActions.CLICK);
            //TestButton(leftHandDevices, CommonUsages.gripButton,            XRDeviceEventTypes.left_grip,           XRDeviceActions.CLICK);
            TestButton(leftHandDevices, CommonUsages.primaryTouch,          XRDeviceEventTypes.left_primary,        XRDeviceActions.TOUCH);
            TestButton(leftHandDevices, CommonUsages.secondaryTouch,        XRDeviceEventTypes.left_secondary,      XRDeviceActions.TOUCH);
            TestButton(leftHandDevices, CommonUsages.primary2DAxisClick,    XRDeviceEventTypes.left_thumbstick,     XRDeviceActions.CLICK);
            TestButton(leftHandDevices, CommonUsages.primary2DAxisTouch,    XRDeviceEventTypes.left_thumbstick,     XRDeviceActions.TOUCH);
            TestButton(leftHandDevices, CommonUsages.menuButton,            XRDeviceEventTypes.menu_button,         XRDeviceActions.CLICK);

            //TestButton(rightHandDevices, CommonUsages.triggerButton,        XRDeviceEventTypes.right_trigger,       XRDeviceActions.CLICK);
            TestButton(rightHandDevices, CommonUsages.primaryButton,        XRDeviceEventTypes.right_primary,       XRDeviceActions.CLICK);
            TestButton(rightHandDevices, CommonUsages.secondaryButton,      XRDeviceEventTypes.right_secondary,     XRDeviceActions.CLICK);
            //TestButton(rightHandDevices, CommonUsages.gripButton,           XRDeviceEventTypes.right_grip,          XRDeviceActions.CLICK);
            TestButton(rightHandDevices, CommonUsages.primaryTouch,         XRDeviceEventTypes.right_primary,       XRDeviceActions.TOUCH);
            TestButton(rightHandDevices, CommonUsages.secondaryTouch,       XRDeviceEventTypes.right_secondary,     XRDeviceActions.TOUCH);
            TestButton(rightHandDevices, CommonUsages.primary2DAxisClick,   XRDeviceEventTypes.right_thumbstick,    XRDeviceActions.CLICK);
            TestButton(rightHandDevices, CommonUsages.primary2DAxisTouch,   XRDeviceEventTypes.right_thumbstick,    XRDeviceActions.TOUCH);

            // Check the status of the joystick movements
            TestThumbStick(leftHandDevices, CommonUsages.primary2DAxis,     XRDeviceEventTypes.left_thumbstick);
            TestThumbStick(rightHandDevices, CommonUsages.primary2DAxis,    XRDeviceEventTypes.right_thumbstick);

            // Check the status of the grip and trigger values
            TestValue(leftHandDevices, CommonUsages.trigger,                XRDeviceEventTypes.left_trigger);
            TestValue(leftHandDevices, CommonUsages.grip,                   XRDeviceEventTypes.left_grip);
            TestValue(rightHandDevices, CommonUsages.trigger,               XRDeviceEventTypes.right_trigger);
            TestValue(rightHandDevices, CommonUsages.grip,                  XRDeviceEventTypes.right_grip);
        }
        else
        {
            // Probably running in standalone mode, so check keyboard instead
            // Right Hand Thumbstick using arrow keys
            if (Input.GetKey(KeyCode.UpArrow)) SendVectorEvent(XRDeviceEventTypes.right_thumbstick, new Vector2(0, 1));
            if (Input.GetKeyUp(KeyCode.UpArrow)) SendVectorEvent(XRDeviceEventTypes.right_thumbstick, new Vector2(0, 0));

            if (Input.GetKey(KeyCode.DownArrow)) SendVectorEvent(XRDeviceEventTypes.right_thumbstick, new Vector2(0, -1));
            if (Input.GetKeyUp(KeyCode.DownArrow)) SendVectorEvent(XRDeviceEventTypes.right_thumbstick, new Vector2(0, 0));

            if (Input.GetKey(KeyCode.LeftArrow)) SendVectorEvent(XRDeviceEventTypes.right_thumbstick, new Vector2(-1, 0));
            if (Input.GetKeyUp(KeyCode.LeftArrow)) SendVectorEvent(XRDeviceEventTypes.right_thumbstick, new Vector2(0, 0));
            
            if (Input.GetKey(KeyCode.RightArrow)) SendVectorEvent(XRDeviceEventTypes.right_thumbstick, new Vector2(1, 0));
            if (Input.GetKeyUp(KeyCode.RightArrow)) SendVectorEvent(XRDeviceEventTypes.right_thumbstick, new Vector2(0, 0));

            // Left Hand Thumbstick using wasd keys
            if (Input.GetKey(KeyCode.W)) SendVectorEvent(XRDeviceEventTypes.left_thumbstick, new Vector2(0, 1));
            if (Input.GetKeyUp(KeyCode.W)) SendVectorEvent(XRDeviceEventTypes.left_thumbstick, new Vector2(0, 0));

            if (Input.GetKey(KeyCode.S)) SendVectorEvent(XRDeviceEventTypes.left_thumbstick, new Vector2(0, -1));
            if (Input.GetKeyUp(KeyCode.S)) SendVectorEvent(XRDeviceEventTypes.left_thumbstick, new Vector2(0, 0));

            if (Input.GetKey(KeyCode.A)) SendVectorEvent(XRDeviceEventTypes.left_thumbstick, new Vector2(-1, 0));
            if (Input.GetKeyUp(KeyCode.A)) SendVectorEvent(XRDeviceEventTypes.left_thumbstick, new Vector2(0, 0));
            
            if (Input.GetKey(KeyCode.D)) SendVectorEvent(XRDeviceEventTypes.left_thumbstick, new Vector2(1, 0));
            if (Input.GetKeyUp(KeyCode.D)) SendVectorEvent(XRDeviceEventTypes.left_thumbstick, new Vector2(0, 0));

            // Mouse button left button = right hand trigger and right button = right hand grip
            if (Input.GetMouseButtonDown(0)) SendKeyEvent(XRDeviceEventTypes.right_trigger, XRDeviceActions.CLICK, true);
            if (Input.GetMouseButtonUp(0)) SendKeyEvent(XRDeviceEventTypes.right_trigger, XRDeviceActions.CLICK, false);

            if (Input.GetMouseButtonDown(1)) SendKeyEvent(XRDeviceEventTypes.right_grip, XRDeviceActions.CLICK, true);
            if (Input.GetMouseButtonUp(1)) SendKeyEvent(XRDeviceEventTypes.right_grip, XRDeviceActions.CLICK, false);

            // Mouse scroll is a special case, useful for things like turning and scrolling where hand movement might be otherwise used
            if (Input.mouseScrollDelta.y > 0) SendMouseEvent(XRDeviceEventTypes.mouse_scroll, XRDeviceActions.UP);
            if (Input.mouseScrollDelta.y < 0) SendMouseEvent(XRDeviceEventTypes.mouse_scroll, XRDeviceActions.DOWN);
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Some useful static helper functions
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public static class XRRig
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Find the current Event Queue, if there is one.  Returns null if there isn't one, or the queue if it finds it.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public static XRDeviceEvents EventQueue
    {
        get
        {
            // Find the object that has the event manager on it.  It should be the only one with tag XREvents.
            GameObject watcherGameObject = GameObject.FindWithTag(Enum.GetName(typeof(OpenXR_UX_Tags), OpenXR_UX_Tags.XREvents));
            if (watcherGameObject == null)
            {
                Debug.Log("There is no XRRig Manager in the SceneGraph that is tagged XREvents.");
                return null;
            }
            else
            {
                XRRig_Manager watcher = watcherGameObject.GetComponent<XRRig_Manager>();
                if (watcher == null)
                {
                    return null;
                }
                else
                {
                    return watcher.XREventQueue;
                }
            }
        }
    }


    public static bool XRDataTest (XREvent theEvent, XRDeviceEventTypes eventToTrigger, XRDeviceActions eventAction)
    {
        return ((theEvent.eventType == XRDeviceEventTypes.mouse_scroll) && (theEvent.eventAction == XRDeviceActions.UP));
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
