/**********************************************************************************************************************************************************
 * XRUX_Knob
 * ---------
 *
 * 2021-08-27
 *
 * A rotating knob that works with OpenXR
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Public functions
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public interface IXRUX_Knob
{
    void Input(XRData newData);
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[AddComponentMenu("OpenXR UX/Objects/XRUX Knob")]
public class XRUX_Knob : MonoBehaviour, IXRUX_Knob
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public XRData.Mode mode = XRData.Mode.User; // For use in the inspector only
    public Renderer objectToColor; // The GameObject for the base of the knob - the one that needs to change colour and move when turned
    public GameObject objectToMove; // The GameObject that will be rotated when clicked and turned
    public Material normalMaterial; // The material for when not pressed
    public Material activatedMaterial; // The material for when pressed
    public Material touchedMaterial; // The material for when touched
    public float maxValue = 10.0f; // The maximum value of the scale
    public float step = 1.0f; // The value by which the knob 'clicks' around
    public UnityXRDataEvent onChange; // Functions to call when the knob is being turned
    public UnityEvent onTouch; // Functions to call when first touched
    public UnityEvent onUntouch; // Functions to call when no longer touched
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private bool touched = false; // Touched or not
    private bool clicked = false; // Clicked or not
    private float touchTime; // Time when last touched - used to make sure the button resets if touch-up doesn't occur - can happen occasionally.
    private Collider otherCollider; // The collider that hits the knob - we need this to determine the rotation.  Usually one of the pointers.
    private float clickedOnValue = 0.0f; // What the value was when the knob is clicked on.
    private float startRotation = 0.0f; // The starting rotation of the other collider.
    private float currentRotation = 0.0f; // The current angle of rotation (degrees).
    private float prevSteppedValue = 0.0f; // The previous value sent as an event.
    private bool firstTime = true; // First time running?
    private bool isLeft = false;
    private bool isRight = false;
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Rotate the knob to the given value (in the range given by the 0, max and step values)
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void Input (XRData newData)
    {
        float newValue = newData.ToFloat();
        bool quietly = newData.quietly;

        // Make it 'stepped'
        float steppedValue = Mathf.Floor(newValue / step) * step;

        // Take that stepped value and work it back to a degree for the knob to give the 'click' effect.
        float knobPosition = (1.0f - (steppedValue / maxValue)) * 360.0f;

        // Rotate the knob
        objectToMove.transform.localRotation = Quaternion.Euler(0, 0, knobPosition);

        // Send the event when a change has occured.
        if ((prevSteppedValue != steppedValue) && !quietly)
        {
            if (onChange != null) onChange.Invoke(new XRData(steppedValue));
        }

        // Save the value for next time
        currentRotation = prevSteppedValue = steppedValue;
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set up the link to the event manager
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        // Listen for events coming from the XR Controllers and other devices
        if (XRRig.EventQueue != null) XRRig.EventQueue.AddListener(onDeviceEvent);

        objectToColor.material = normalMaterial;
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // It is possible that a knob may be touched but the system misses the OnTriggerExit event, and it stays touched.  Therefore, after a small amount of
    // time after the last OnTriggerStay, take the knob back to the Up state.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        if (((Time.time - touchTime) > 0.1) && touched && !clicked)
        {
            touched = false;
            objectToColor.material = normalMaterial;
            otherCollider = null;

            if (onUntouch != null) onUntouch.Invoke();
            isLeft = isRight = false;
        }

        if (clicked)
        {
            if (otherCollider != null)
            {
                Debug.Log("Rotating");
                // Get the angle of rotation of the collided item (usually a pointer)
                currentRotation = clickedOnValue + (otherCollider.gameObject.transform.rotation.eulerAngles.z - startRotation) * 2.0f;

                // Normalise that value (to between 0 and 1)
                float normalizedValue = (1.0f - (currentRotation % 360.0f) / 360.0f);
                // Limit it to between min and max
                float limitedValue = (normalizedValue * maxValue) % maxValue;

                // Make it 'stepped'
                float steppedValue = Mathf.Floor(limitedValue / step) * step;

                // Take that stepped value and work it back to a degree for the knob to give the 'click' effect.
                float knobPosition = (1.0f - (steppedValue / maxValue)) * 360.0f;

                // Rotate the knob
                objectToMove.transform.localRotation = Quaternion.Euler(0, 0, knobPosition);
                // Send the event when a change has occured.
                if ((prevSteppedValue != steppedValue) || firstTime)
                {
                    if (onChange != null) onChange.Invoke(new XRData(steppedValue));
                }

                // Save the value for next time
                prevSteppedValue = steppedValue;

                firstTime = false;
            }
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set the knob to normal state when being activated
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void OnDisable()
    {
        touched = clicked = false;
        objectToColor.material = normalMaterial;
    }
    void OnEnable()
    {
        touched = clicked = false;
        objectToColor.material = normalMaterial;
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // What to do when the knob collider is triggered or untriggered.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (!clicked)
        {
            otherCollider = other;
            if (other.gameObject.tag == "XRLeft") isLeft = true;
            if (other.gameObject.tag == "XRRight") isRight = true;
            touched = true;
            objectToColor.material = touchedMaterial;
            if (onTouch != null) onTouch.Invoke();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        touchTime = Time.time;
        if (other.gameObject.tag == "XRLeft") isLeft = true;
        if (other.gameObject.tag == "XRRight") isRight = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (!clicked)
        {
            touched = false;
            objectToColor.material = normalMaterial;
            isLeft = isRight = false;
            if (onUntouch != null) onUntouch.Invoke();
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Once the knob is entered, what to do when one of the triggers is pressed.
    // TODO: Check that the trigger pressed is the one whose pointer has activated the knob.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void onDeviceEvent(XREvent theEvent)
    {
        if ((((theEvent.eventType == XRDeviceEventTypes.left_trigger) && isLeft) || ((theEvent.eventType == XRDeviceEventTypes.right_trigger) && isRight)) && 
        (theEvent.eventAction == XRDeviceActions.CLICK) && touched)
        {
            if (theEvent.eventBool)
            {
                clicked = true;
                objectToColor.material = activatedMaterial;
                clickedOnValue = currentRotation;
                startRotation = otherCollider.gameObject.transform.rotation.eulerAngles.z;
            }
            else
            {
                clicked = false;
                objectToColor.material = touchedMaterial;
            }
        }

        if ((theEvent.eventType == XRDeviceEventTypes.mouse_scroll) && (theEvent.eventAction == XRDeviceActions.UP) && touched)
        {
            float newSteppedValue = prevSteppedValue + step;
            Input(new XRData((newSteppedValue >= maxValue) ? 0 : newSteppedValue));
        }
        if ((theEvent.eventType == XRDeviceEventTypes.mouse_scroll) && (theEvent.eventAction == XRDeviceActions.DOWN) && touched)
        {
            float newSteppedValue = prevSteppedValue - step;
            Input(new XRData((newSteppedValue < 0) ? maxValue : newSteppedValue));
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
