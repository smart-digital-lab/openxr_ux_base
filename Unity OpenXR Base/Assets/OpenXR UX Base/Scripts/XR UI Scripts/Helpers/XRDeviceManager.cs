/**********************************************************************************************************************************************************
 * XRDeviceManager
 * ---------------
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
    _
};
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Action modifiers for the Events above.
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public enum XRDeviceActions
{
    TOUCH, CLICK, MOVE, LOOKAT, POINTAT, _
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
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// The unity event queue for XR events
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[Serializable]
public class XRDeviceEvents : UnityEvent<XREvent> { };
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Other events that are sent between XR Modules
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public enum XRDataType { INT, FLOAT, BOOL, STRING }
[Serializable]
public class XRData
{
    private XRDataType theType;
    private int intValue = 0;
    private float floatValue = 0.0f;
    private bool boolValue = false;
    private string stringValue = "";
    private bool quietlyValue = false;

    public bool quietly { get { return quietlyValue; }  set { quietlyValue = value; } }

    public new XRDataType GetType() { return theType; }
    public int ToInt() { return intValue; }
    public float ToFloat() { return floatValue; }
    public bool ToBool() { return boolValue; }
    public override string ToString() {return stringValue; }

    public XRData(float newValue, bool newQuietly = false)
    {
        intValue = Mathf.RoundToInt(newValue);
        floatValue = newValue;
        boolValue = Convert.ToBoolean(newValue);
        stringValue = newValue.ToString();
        quietly = newQuietly;
        theType = XRDataType.FLOAT;
    }
    public XRData(int newValue, bool newQuietly = false)
    {
        intValue = newValue;
        floatValue = Convert.ToSingle(newValue);
        boolValue = Convert.ToBoolean(newValue);
        stringValue = newValue.ToString();
        quietly = newQuietly;
        theType = XRDataType.INT;
    }
    public XRData(bool newValue, bool newQuietly = false)
    {
        intValue = Convert.ToInt32(newValue);
        floatValue = Convert.ToSingle(newValue);
        boolValue = newValue;
        stringValue = newValue.ToString();
        quietly = newQuietly;
        theType = XRDataType.BOOL;
    }
    public XRData(string newValue, bool newQuietly = false)
    {
        int.TryParse(newValue, out intValue);
        float.TryParse(newValue, out floatValue);
        bool.TryParse(newValue, out boolValue);
        stringValue = newValue;
        quietly = newQuietly;
        theType = XRDataType.STRING;
    }
}

[Serializable]
public class UnityXRDataEvent : UnityEvent<XRData> {}
[Serializable]
public class UnityIntegerEvent : UnityEvent<int> {}
[Serializable]
public class UnityBooleanEvent : UnityEvent<bool> {}
[Serializable]
public class UnityFloatEvent : UnityEvent<float> {}
[Serializable]
public class UnityStringEvent : UnityEvent<string> {}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Public functions
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public interface _XRDeviceManager
{
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public class XRDeviceManager : MonoBehaviour, _XRDeviceManager
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    [Header("____________________________________________________________________________________________________")]
    [Header("The Smart Digital Lab OpenXR UX Base\n____________________________________________________________________________________________________")]
    [Header("INPUTS\n\n - Data from the openXR Controllers.")]

    [Header("____________________________________________________________________________________________________")]
    [Header("SETTINGS")]
    [Header("____________________________________________________________________________________________________")]
    [Header("OUTPUTS")]
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
    void TestButton(List<InputDevice> XRdevices, InputFeatureUsage<bool> usage, XRDeviceEventTypes eventToTrigger, XRDeviceActions eventAction)
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



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Each frame, check for any new actions that need sending out.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        TestForDevices();

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
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
