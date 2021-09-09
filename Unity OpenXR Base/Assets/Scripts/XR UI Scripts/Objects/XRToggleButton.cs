/**********************************************************************************************************************************************************
 * XRToggleButton
 * --------------
 *
 * 2021-08-25
 *
 * A button that works with OpenXR that goes on when pressed once, and goes off again when pressed again
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Microsoft.CSharp;

enum ButtonToggleStates { Up, Down }; // Toggle button states.

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Public functions
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public interface _XRToggleButton
{
    void Title(string newTitle); // Change the text on the button
    void Title(int newTitle); // Change the text on the button
    void Title(float newTitle); // Change the text on the button
    void Title(bool newTitle); // Change the text on the button

    void Input(XRData newData); // Set the state of the button.  If quietly is set to true, doesn't invoke the callbacks.
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public class XRToggleButton : MonoBehaviour, _XRToggleButton
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    [Header("____________________________________________________________________________________________________")]
    [Header("An on/off toggle button that integrates with the OpenXR UX base\n____________________________________________________________________________________________________")]
    [Header("INPUTS\n\n - Title( [ int | float | bool | string ] ) - Set the button title.\n - Input( XRData ) - Boolean value to toggle the button as if it was being pressed.")]

    [Header("____________________________________________________________________________________________________")]
    [Header("SETTINGS")]

    [Header("The object that will change colour when pressed.")]
    public Renderer baseRenderer; // The GameObject for the base of the button - the one that needs to change colour and move when pressed
    [Header("The object that will move when pressed.")]
    public GameObject pusher; // The GameObject that will move when the button is activated

    [Header("Materials for the different interactions states.")]
    public Material normalMaterial; // The material for when not pressed
    public Material activatedMaterial; // The material for when pressed
    public Material touchedMaterial; // The material for when touched

    [Header("____________________________________________________________________________________________________")]
    [Header("OUTPUTS")]
    public UnityXRDataEvent onChange; // Functions to call when the button is toggled on or off
    public UnityEvent onTouch; // Functions to call when first touched
    public UnityEvent onUntouch; // Functions to call when no longer touched
    public UnityEvent onToggleOn; // Functions to call when the button is toggled on
    public UnityEvent onToggleOff; // Functions to call when the button is toggled off
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private XRDeviceManager watcher; // The XR Event Manager
    private ButtonToggleStates buttonState; // The current state of the toggle button
    private bool touched = false; // Whether touched or not
    private float touchTime; // Time when last touched - used to make sure the button resets if touch-up doesn't occur - can happen occasionally.
    private Vector3 startPosition; //Stores the start position at startup so it can be used for the 'out' position.
    private bool isLeft = false; // Keeps tracks of whether the left or right controller has touched the button for when clicking occurs.
    private bool isRight = false;
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Change the text on the button
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void Title(float newTitle) { Title(newTitle.ToString()); }
    public void Title(int newTitle) { Title(newTitle.ToString()); }
    public void Title(bool newTitle) { Title(newTitle.ToString()); }
    public void Title(string newTitle)
    {
        ChangeTextOnTMP textToChange = GetComponentInChildren<ChangeTextOnTMP>();
        if (textToChange != null) textToChange.Input(newTitle);
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Change the state of the button
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void Input(XRData newData) { Set(newData.ToBool(), newData.quietly); }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------




    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set up the link to the event manager
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

        baseRenderer.material = normalMaterial;
        startPosition = pusher.transform.localPosition;
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // It is possible that a button may be touched but the system misses the OnTriggerExit event, and it stays touched.  Therefore, after a small amount of
    // time after the last OnTriggerStay, take the button back to the Up state.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        if (((Time.time - touchTime) > 0.1) && touched)
        {
            if (onUntouch != null) onUntouch.Invoke();

            touched = false;
            if (buttonState == ButtonToggleStates.Up)
            {
                pusher.transform.localPosition = startPosition;
                baseRenderer.material = normalMaterial;
            }
            else if (buttonState == ButtonToggleStates.Down)
            {
                baseRenderer.material = activatedMaterial;
            }
            isLeft = isRight = false;
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void OnDisable()
    {
    }
    void OnEnable()
    {
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------




    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // What to do when the button collider is triggered or untriggered.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "XRLeft") isLeft = true;
        if (other.gameObject.tag == "XRRight") isRight = true;
        touched = true;
        baseRenderer.material = touchedMaterial;
        if (onTouch != null) onTouch.Invoke();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "XRLeft") isLeft = true;
        if (other.gameObject.tag == "XRRight") isRight = true;
        touched = true;
        touchTime = Time.time;
    }
    private void OnTriggerExit(Collider other)
    {
        touched = false;
        if (onUntouch != null) onUntouch.Invoke();

        if (buttonState == ButtonToggleStates.Up)
        {
            baseRenderer.material = normalMaterial;
        }
        else if (buttonState == ButtonToggleStates.Down)
        {
            baseRenderer.material = activatedMaterial;
        }
        isLeft = isRight = false;
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Once the button is entered, what to do when one of the triggers is pressed.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void onDeviceEvent(XREvent theEvent)
    {
        if ((((theEvent.eventType == XRDeviceEventTypes.left_trigger) && isLeft) || ((theEvent.eventType == XRDeviceEventTypes.right_trigger) && isRight)) && 
        (theEvent.eventAction == XRDeviceActions.CLICK) && (touched))       
        {
            if ((buttonState == ButtonToggleStates.Up) && (theEvent.eventBool))
            {
                // Radio switch On
                Set (true);
            }
            else if ((buttonState == ButtonToggleStates.Down) && (theEvent.eventBool))
            {
                // Radio switch Off
                Set (false);
            }
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set the Toggle Button.  Can also be called from other functions (such as for creating a stack of buttons).
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Set(bool newValue, bool quietly = false)
    {
        if (newValue)
        {
            // Toggle On
            buttonState = ButtonToggleStates.Down;
            baseRenderer.material = activatedMaterial;
            pusher.transform.localPosition = new Vector3(startPosition.x, startPosition.y, startPosition.z + 0.004f);
            if ((onToggleOn != null) && !quietly) onToggleOn.Invoke();
        }
        else
        {
            // Toggle Off
            buttonState = ButtonToggleStates.Up;
            baseRenderer.material = normalMaterial;
            pusher.transform.localPosition = startPosition;
            if ((onToggleOff != null) && !quietly) onToggleOff.Invoke();
        }

        if ((onChange != null) && !quietly) onChange.Invoke(new XRData(newValue));
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
