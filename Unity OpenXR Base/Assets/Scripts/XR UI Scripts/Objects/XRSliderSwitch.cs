/******************************************************************************************************************************************************
 * XRSliderSwitch
 * --------------
 *
 * 2021-08-30
 *
 * A slider switch
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 ******************************************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

enum SliderSwitchStates { Up, Down }; // Slider Switch states.

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Public functions
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public interface _XRSliderSwitch
{
    void Title(string newTitle); // Change the text on the side
    void Title(int newTitle); // Change the text on the side
    void Title(float newTitle); // Change the text on the side
    void Title(bool newTitle); // Change the text on the side

    void Input(XRData newData); // Set the state of the switch.  If quietly is set to true, doesn't invoke the callbacks.

    string Title(); // Return the title of the switch
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[AddComponentMenu("XR/UX/Objects/XRSliderSwitch")]
public class XRSliderSwitch : MonoBehaviour, _XRSliderSwitch
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    [Header("____________________________________________________________________________________________________")]
    [Header("A Slider Switch that can be in one of two states.\n____________________________________________________________________________________________________")]
    [Header("INPUTS\n\n - Title( [ int | float | bool | string ] ) - Set the button title.\n - Input( XRData ) - Boolean value to change the switch state as if it was being pressed.")]

    [Header("____________________________________________________________________________________________________")]
    [Header("SETTINGS")]
    [Header("The object that will change colour when touched and clicked.")]
    public Renderer baseRenderer; // The GameObject for the base of the switch - the one that needs to change colour and move when pressed

    [Header("Materials for the different interactions states.")]
    public Material normalMaterial; // The material for when not pressed
    public Material activatedMaterial; // The material for when pressed
    public Material touchedMaterial; // The material for when touched

    [Header("____________________________________________________________________________________________________")]
    [Header("OUTPUTS")]
    public UnityXRDataEvent onChange;
    public UnityEvent onTouch; // Functions to call when first touched
    public UnityEvent onUntouch; // Functions to call when no longer touched
    public UnityEvent onSliderOn; // Functions to call when the switch is toggled on
    public UnityEvent onSliderOff; // Functions to call when the switch is toggled off
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private XRDeviceManager watcher; // The XR Event Manager
    private SliderSwitchStates switchState; // The current state of the toggle switch
    private bool touched = false; // Whether touched or not
    private float touchTime; // Time when last touched - used to make sure the switch resets if touch-up doesn't occur - can happen occasionally.
    private Vector3 startPosition;
    private bool isLeft = false;
    private bool isRight = false;
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Change the text on the switch
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
    // Change the state of the given switch
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
    // It is possible that a switch may be touched but the system misses the OnTriggerExit event, and it stays touched.  Therefore, after a small amount of
    // time after the last OnTriggerStay, take the switch back to the Up state.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        if (((Time.time - touchTime) > 0.1) && touched)
        {
            if (onUntouch != null) onUntouch.Invoke();

            touched = false;
            if (switchState == SliderSwitchStates.Up)
            {
                baseRenderer.material = normalMaterial;
            }
            else if (switchState == SliderSwitchStates.Down)
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
    // What to do when the switch collider is triggered or untriggered.
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

        if (switchState == SliderSwitchStates.Up)
        {
            baseRenderer.material = normalMaterial;
        }
        else if (switchState == SliderSwitchStates.Down)
        {
            baseRenderer.material = activatedMaterial;
        }
        isLeft = isRight = false;
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Once the switch is entered, what to do when one of the triggers is pressed.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void OnDeviceEvent(XREvent theEvent)
    {
         if ((((theEvent.eventType == XRDeviceEventTypes.left_trigger) && isLeft) || ((theEvent.eventType == XRDeviceEventTypes.right_trigger) && isRight)) && 
        (theEvent.eventAction == XRDeviceActions.CLICK) && (touched))       
        {
            if ((switchState == SliderSwitchStates.Up) && (theEvent.eventBool))
            {
                // Radio switch On
                Set (true);
            }
            else if ((switchState == SliderSwitchStates.Down) && (theEvent.eventBool))
            {
                // Radio switch Off
                Set (false);
            }
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set the slider switch.  Can also be called from other functions (such as for creating a stack of switches).
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Set(bool newValue, bool quietly = false)
    {
        if (newValue)
        {
            // Radio switch On
            switchState = SliderSwitchStates.Down;
            baseRenderer.material = activatedMaterial;
            baseRenderer.gameObject.transform.localPosition = new Vector3(startPosition.x, startPosition.y, startPosition.z - 0.01f);
            if ((onSliderOn != null) && !quietly) onSliderOn.Invoke();
        }
        else
        {
            // Radio switch Off
            switchState = SliderSwitchStates.Up;
            baseRenderer.material = normalMaterial;
            baseRenderer.gameObject.transform.localPosition = startPosition;
            if ((onSliderOff != null) && !quietly) onSliderOff.Invoke();
        }
        if ((onChange != null) && !quietly) onChange.Invoke(new XRData(newValue));
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
