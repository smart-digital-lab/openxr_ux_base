/******************************************************************************************************************************************************
 * XRRadioButton
 * --------------
 *
 * 2021-08-29
 *
 * A single button to form part of a set of radiobuttons
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 ******************************************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Public functions
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public interface _XRRadioButton
{
    void Title(string newTitle); // Change the text on the side
    void Title(int newTitle); // Change the text on the side
    void Title(float newTitle); // Change the text on the side
    void Title(bool newTitle); // Change the text on the side

    void Input(XRData newData); // Set the state of the radio button.  If quietly is set to true, doesn't invoke the callbacks.

    string Title(); // Return the title of the radio button
    bool ButtonState(); // Return the current state of the radio button
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[AddComponentMenu("OpenXR UX/Objects/XRRadioButton")]
public class XRRadioButton : MonoBehaviour, _XRRadioButton
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    [Header("____________________________________________________________________________________________________")]
    [Header("A Radio Button that integrates with the OpenXR UX base \nand can be used as part of a set of Radio Buttons.\n____________________________________________________________________________________________________")]
    [Header("INPUTS\n\n - Clear() - Title( [ int | float | bool | string ] ) - Set the button title.\n - Input( XRData ) - Boolean value to change the button state as if it was being pressed.")]

    [Header("____________________________________________________________________________________________________")]
    [Header("SETTINGS")]

    [Header("The object that will change colour when touched and rotated.")]
    public Renderer baseRenderer; // The GameObject for the base of the button - the one that needs to change colour and move when pressed

    [Header("Materials for the different interactions states.")]
    public Material normalMaterial; // The material for when not pressed
    public Material activatedMaterial; // The material for when pressed
    public Material touchedMaterial; // The material for when touched

    [Header("Starting state")]
    public bool startState = false;

    [Header("____________________________________________________________________________________________________")]
    [Header("OUTPUTS")]
    public UnityXRDataEvent onChange; // Functions to call when the button is toggled on or off (with the bool value set appropriately)
    public UnityEvent onTouch; // Functions to call when first touched
    public UnityEvent onUntouch; // Functions to call when no longer touched
    public UnityEvent onRadioOn; // Functions to call when the button is toggled on
    public UnityEvent onRadioOff; // Functions to call when the button is toggled off
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private XRDeviceManager watcher; // The XR Event Manager
    private bool buttonState = false; // The current state of the toggle button
    private bool touched = false; // Whether touched or not
    private float touchTime; // Time when last touched - used to make sure the button resets if touch-up doesn't occur - can happen occasionally.
    private Vector3 startPosition; //Stores the start position at startup so it can be used for the 'out' position.
    private bool isLeft = false; // Keeps tracks of whether the left or right controller has touched the button for when clicking occurs.
    private bool isRight = false;
    private bool firstTime = true;
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
    public void Input(XRData newData)
    {
        Set(newData.ToBool(), newData.quietly);
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Return the title of the XR Radio Button
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public string Title()
    {
        ChangeTextOnTMP textToChange = GetComponentInChildren<ChangeTextOnTMP>();
        return ((textToChange == null) ? "" : textToChange.Text());
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public bool ButtonState()
    {
        return buttonState;
    }
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
            watcher.XREventQueue.AddListener(OnDeviceEvent);
        }

        baseRenderer.material = normalMaterial;
        startPosition = baseRenderer.gameObject.transform.localPosition;
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // It is possible that a button may be touched but the system misses the OnTriggerExit event, and it stays touched.  Therefore, after a small amount of
    // time after the last OnTriggerStay, take the button back to the Up state.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        if (firstTime)
        {
            Set(startState, true);
            firstTime = false;
        }

        if (((Time.time - touchTime) > 0.1) && touched)
        {
            if (onUntouch != null) onUntouch.Invoke();

            touched = false;
            if (buttonState)
            {
                baseRenderer.material = activatedMaterial;
            }
            else
            {
                baseRenderer.material = normalMaterial;
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
        touchTime = Time.time;
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

        if (buttonState)
        {
            baseRenderer.material = activatedMaterial;
        }
        else
        {
            baseRenderer.material = normalMaterial;
        }
        isLeft = isRight = false;
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Once the button is entered, what to do when one of the triggers is pressed.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void OnDeviceEvent(XREvent theEvent)
    {
         if ((((theEvent.eventType == XRDeviceEventTypes.left_trigger) && isLeft) || ((theEvent.eventType == XRDeviceEventTypes.right_trigger) && isRight)) && 
        (theEvent.eventAction == XRDeviceActions.CLICK) && (touched))       
        {
            if (theEvent.eventBool)
            {
                // Radio button On or Off
                Set (!buttonState);
            }
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set the radio button.  Can also be called from other functions (such as for creating a stack of radio buttons).
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Set(bool newValue, bool quietly = false)
    {
        if (newValue)
        {
            // Radio button On
            buttonState = true;
            baseRenderer.material = activatedMaterial;
            baseRenderer.gameObject.transform.localPosition = new Vector3(startPosition.x, startPosition.y, startPosition.z + 0.004f);
            if ((onRadioOn != null) && !quietly) onRadioOn.Invoke();
        }
        else
        {
            // Radio button Off
            buttonState = false;
            baseRenderer.material = normalMaterial;
            baseRenderer.gameObject.transform.localPosition = startPosition;
            if ((onRadioOff != null) && !quietly) onRadioOff.Invoke();
        }
        if ((onChange != null) && !quietly) onChange.Invoke(new XRData(newValue));
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
