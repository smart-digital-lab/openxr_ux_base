/**********************************************************************************************************************************************************
 * XRUX_Button
 * -----------
 *
 * 2021-08-25
 *
 * A generic button class inherited by other button classes that provide the core functionality
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Public functions
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public interface IXRUX_Button
{
    void Title(string newTitle);            // Change the text on the button
    void Title(int newTitle);               // Change the text on the button
    void Title(float newTitle);             // Change the text on the button
    void Title(bool newTitle);              // Change the text on the button
    void Title(Vector3 newTitle);           // Change the text on the button
    void Title(XRData newTitle);            // Change the text on the button

    void Input(XRData newdata);             // Set the state of the button.  If quietly is set to true, doesn't invoke the callbacks.
        
    string Title();                         // Return the title of the button
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[AddComponentMenu("OpenXR UX/Objects/XRUX Button")]
public class XRUX_Button : MonoBehaviour, IXRUX_Button
{
    public enum XRGenericButtonAxis { X, Y, Z, None };
    public enum XRGenericButtonMovement { Toggle, Momentary };

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public XRData.Mode mode = XRData.Mode.User; // For use in the inspector only

    public Renderer objectToColor;          // The object that needs to change colour when activated
    public GameObject objectToMove;         // The GameObject that will move when activated
    public Material normalMaterial;         // The material for when not pressed
    public Material activatedMaterial;      // The material for when pressed
    public Material touchedMaterial;        // The material for when touched
    public XRGenericButtonAxis movementAxis = XRGenericButtonAxis.Z;
    public float movementAmount;            // Amount of movement along the selected axis
    public XRGenericButtonMovement movementStyle = XRGenericButtonMovement.Toggle;
    public UnityXRDataEvent onChange;       // Changes on click or unclick, with boolean
    public UnityEvent onClick;              // Functions to call when click-down
    public UnityEvent onUnclick;            // Functions to call when click-up
    public UnityEvent onTouch;              // Functions to call when first touched
    public UnityEvent onUntouch;            // Functions to call when first touched
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private bool buttonState = false;       // Current button state
    private float touchTime;                // Time when last touched - used to make sure the button resets if touch-up doesn't occur - can happen occasionally.
    private Vector3 startPosition;          // Stores the start position at startup so it can be used for the 'off / out' position.
    private bool isLeft = false;            // Keeps tracks of whether the left or right controller has touched the button for when clicking occurs.
    private bool isRight = false;
    private bool touched = false;
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Change the text on the button
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void Title(float newTitle) { Title(newTitle.ToString()); }
    public void Title(int newTitle) { Title(newTitle.ToString()); }
    public void Title(bool newTitle) { Title(newTitle.ToString()); }
    public void Title(Vector3 newTitle) { Title(XRData.FromVector3(newTitle)); }
    public void Title(XRData newTitle) { Title(newTitle.ToString()); }
    public void Title(string newTitle)
    {
        XRUX_SetText textToChange = GetComponentInChildren<XRUX_SetText>();
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
        XRUX_SetText textToChange = GetComponentInChildren<XRUX_SetText>();
        return ((textToChange == null) ? "" : textToChange.Text());
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Save start position and material
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Awake()
    {
        if (objectToColor != null) objectToColor.material = normalMaterial;
        if (objectToMove != null) startPosition = objectToMove.transform.localPosition;       
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set up the link to the event manager
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        // Listen for events coming from the XR Controllers and other devices
        if (XRRig.EventQueue != null) XRRig.EventQueue.AddListener(onDeviceEvent);
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // If the touching object disappears before it stops touching, there is no OnTriggerExit event
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        if (((Time.time - touchTime) > 0.1) && touched)
        {
            if (movementStyle == XRGenericButtonMovement.Momentary)
            {
                touched = false;
                Set(false);
            }
            else
            {
                DoTouchExit();
            }
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------


    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set the button to the correct state when being activated in case it was not active and missed being moved or coloured
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void OnDisable()
    {
        touched = false;
    }
    void OnEnable()
    {
        touched = false;
        if (movementStyle == XRGenericButtonMovement.Momentary)
        {
            Set(false, true);
        }
        else
        {
            Set(buttonState, true);
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // What to do when the button collider is triggered or untriggered (usually by the pointers).
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void OnTriggerEnter(Collider other)
    {
        touched = true;
        if (other.gameObject.tag == "XRLeft") isLeft = true;
        if (other.gameObject.tag == "XRRight") isRight = true;
        if (objectToColor != null) objectToColor.material = touchedMaterial;
        if (onTouch != null) onTouch.Invoke();
    }
    void OnTriggerStay(Collider other)
    {
        touched = true;
        if (other.gameObject.tag == "XRLeft") isLeft = true;
        if (other.gameObject.tag == "XRRight") isRight = true;
        touchTime = Time.time;
    }
    void OnTriggerExit(Collider other)
    {
        DoTouchExit();
    }
    private void DoTouchExit()
    {
        touched = false;
        if (movementStyle == XRGenericButtonMovement.Momentary)
        {
            Set(false);
        }
        else
        {
            if (buttonState)
            {
                if (objectToColor != null) objectToColor.material = activatedMaterial;
            }
            else
            {
                if (objectToColor != null) objectToColor.material = normalMaterial;
            }
        }
        isLeft = isRight = false;
        if (onUntouch != null) onUntouch.Invoke();
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Once the button is touched, what to do when one of the triggers is pressed.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void onDeviceEvent(XREvent theEvent)
    {
        if ((((theEvent.eventType == XRDeviceEventTypes.left_trigger) && isLeft) || ((theEvent.eventType == XRDeviceEventTypes.right_trigger) && isRight)) && 
        (theEvent.eventAction == XRDeviceActions.CLICK) && touched)
        {
            if (movementStyle == XRGenericButtonMovement.Momentary)
            {
                Set(theEvent.eventBool);
            }
            else
            {
                if (theEvent.eventBool)
                {
                    Set(!buttonState);
                }
            }
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set the button.  Can also be called from other functions via the Input function.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Set(bool newButtonState, bool quietly = false)
    {
        buttonState = newButtonState;

        if (buttonState)
        {
            if (objectToColor != null) objectToColor.material = activatedMaterial;
            if (objectToMove != null) 
            {
                Vector3 localSize = gameObject.transform.localScale;
                XRUX_Button_Sizer sizerScript = gameObject.GetComponent<XRUX_Button_Sizer>();
                if (sizerScript != null)
                {
                    localSize = sizerScript.objectToResize.transform.localScale;
                }
                switch (movementAxis)
                {
                    case XRGenericButtonAxis.X:
                        objectToMove.transform.localPosition = new Vector3(startPosition.x + localSize.x * movementAmount, startPosition.y, startPosition.z);
                        break;
                    case XRGenericButtonAxis.Y:
                        objectToMove.transform.localPosition = new Vector3(startPosition.x, startPosition.y + localSize.y * movementAmount, startPosition.z);
                        break;
                    case XRGenericButtonAxis.Z:                       
                        objectToMove.transform.localPosition = new Vector3(startPosition.x, startPosition.y, startPosition.z + localSize.z * movementAmount);
                        break;
                    default:
                        break;
                }
                
            }
            if ((onClick != null) && !quietly) onClick.Invoke();
        }
        else
        {
            if (objectToColor != null) objectToColor.material = normalMaterial;
            if (objectToMove != null) objectToMove.transform.localPosition = startPosition;
            if ((onUnclick != null) && !quietly) onUnclick.Invoke();
        }
        
        if ((onChange != null) && !quietly) onChange.Invoke(new XRData(buttonState));
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
