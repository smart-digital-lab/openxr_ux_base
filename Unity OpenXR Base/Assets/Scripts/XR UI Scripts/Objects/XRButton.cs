/**********************************************************************************************************************************************************
 * XRButton
 * --------
 *
 * 2021-08-25
 *
 * A button that works with OpenXR and is intended to be used with the XR Button prefab.
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

enum ButtonStates { Up, Down, Touched };

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Public functions
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public interface _XRButton
{
    void Title(string newTitle); // Change the text on the button
    void Title(int newTitle); // Change the text on the button
    void Title(float newTitle); // Change the text on the button
    void Title(bool newTitle); // Change the text on the button

    void Input(XRData newdata); // Set the state of the button.  If quietly is set to true, doesn't invoke the callbacks.
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[AddComponentMenu("OpenXR UX/Objects/XRButton")]
public class XRButton : MonoBehaviour, _XRButton
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    [Header("____________________________________________________________________________________________________")]
    [Header("A pushable button that integrates with the OpenXR UX base.\n____________________________________________________________________________________________________")]
    [Header("INPUTS\n\n - Title( [ int | float | bool | string ] ) - Set the button title.\n - Input( XRData ) - Boolean value to change the button state as if it was being pressed.")]

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

    [Header("Whether to move or not.")]
    public bool moveWhenClicked = true;

    [Header("____________________________________________________________________________________________________")]
    [Header("OUTPUTS")]
    public UnityXRDataEvent onChange; // Changes on click or unclick, with boolean
    public UnityEvent onClick; // Functions to call when click-down
    public UnityEvent onUnclick; // Functions to call when click-up
    public UnityEvent onTouch; // Functions to call when first touched
    public UnityEvent onUntouch; // Functions to call when no longer touched
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private XRDeviceManager watcher; // The XR Event Manager
    private ButtonStates buttonState; // Current button state
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
    public void Input(XRData newData)
    {
        Set(newData.ToBool(), newData.quietly);
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
            watcher.XREventQueue.AddListener(onDeviceEvent);
        }
        baseRenderer.material = normalMaterial;
        if (pusher != null) startPosition = pusher.transform.localPosition;
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // It is possible that a button may be touched but the system misses the OnTriggerExit event, and it stays touched.  Therefore, after a small amount of
    // time after the last OnTriggerStay, take the button back to the Up state.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        if (((Time.time - touchTime) > 0.1) && (buttonState != ButtonStates.Up))
        {
            if (buttonState == ButtonStates.Down)
            {
                if (onUnclick != null) onUnclick.Invoke();
            }
            else if (buttonState == ButtonStates.Touched)
            {
                if (onUntouch != null) onUntouch.Invoke();
            }

            buttonState = ButtonStates.Up;
            baseRenderer.material = normalMaterial;
            if (pusher != null) pusher.transform.localPosition = startPosition;
            isLeft = isRight = false;
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------


    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set the button to up state when being activated
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void OnDisable()
    {
        buttonState = ButtonStates.Up;
        baseRenderer.material = normalMaterial;
    }
    void OnEnable()
    {
        buttonState = ButtonStates.Up;
        baseRenderer.material = normalMaterial;
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // What to do when the button collider is triggered or untriggered (usually by the pointers).
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "XRLeft") isLeft = true;
        if (other.gameObject.tag == "XRRight") isRight = true;
        buttonState = ButtonStates.Touched;
        baseRenderer.material = touchedMaterial;
        if (onTouch != null) onTouch.Invoke();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "XRLeft") isLeft = true;
        if (other.gameObject.tag == "XRRight") isRight = true;
        buttonState = ButtonStates.Touched;
        touchTime = Time.time;
    }
    private void OnTriggerExit(Collider other)
    {
        buttonState = ButtonStates.Up;
        baseRenderer.material = normalMaterial;
        if (pusher != null) pusher.transform.localPosition = startPosition;
        isLeft = isRight = false;
        if (onUntouch != null) onUntouch.Invoke();
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Once the button is entered, what to do when one of the triggers is pressed.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void onDeviceEvent(XREvent theEvent)
    {
        if ((((theEvent.eventType == XRDeviceEventTypes.left_trigger) && isLeft) || ((theEvent.eventType == XRDeviceEventTypes.right_trigger) && isRight)) && 
        (theEvent.eventAction == XRDeviceActions.CLICK) && (buttonState != ButtonStates.Up))
        {
            Set (theEvent.eventBool);
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set the button.  Can also be called from other functions via the Input function.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Set(bool newValue, bool quietly = false)
    {
        if (newValue)
        {
            buttonState = ButtonStates.Down;
            baseRenderer.material = activatedMaterial;
            if (moveWhenClicked && (pusher != null)) pusher.transform.localPosition = new Vector3(startPosition.x, startPosition.y, startPosition.z + 0.004f);
            if ((onClick != null) && !quietly) onClick.Invoke();
            if ((onChange != null) && !quietly) onChange.Invoke(new XRData(true));
        }
        else
        {
            buttonState = ButtonStates.Touched;
            baseRenderer.material = touchedMaterial;
            if (moveWhenClicked && (pusher != null)) pusher.transform.localPosition = startPosition;
            if ((onUnclick != null) && !quietly) onUnclick.Invoke();
            if ((onChange != null) && !quietly) onChange.Invoke(new XRData(false));
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
