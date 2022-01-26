/**********************************************************************************************************************************************************
 * XRRig_CameraMover
 * -----------------
 *
 * 2021-09-07
 *
 * Moves the camera under program (ie user) control.  Should be placed on the XRRig or similar.
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Public functions
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public interface IXRRig_CameraMover
{
    void PutOnBrakes();                         // Slow down all movement
    void SetMovementStyle(XRData selection);    // Set the movement style (0 = teleport, 1 = move)
    void SetRotationStyle(XRData selection);    // Set the rotation style (0 = stepped, 1 = smooth)
    void SetRotationAngle(XRData newAngle);     // Set the rotation angle (degrees)
    void StandOnGround();                       // Move the viewpoint to standing on the ground
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public class XRRig_CameraMover : MonoBehaviour, IXRRig_CameraMover
{
    public enum MovementStyle   { teleportToMarker, moveToMarker }
    public enum MovementHand    { Left, Right }
    public enum MovementDevice  { Head, Controller }
    public enum XRType          { Immersive_XR, Desktop_XR }
    public enum RotationStyle   { Stepped, Smooth }

    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public XRData.Mode mode = XRData.Mode.User; // For use in the inspector only
    public MovementStyle movementStyle = MovementStyle.teleportToMarker;
    public GameObject teleportFader;
    public GameObject theHead;
    public GameObject thePlayer;
    public GameObject mainBody;
    public float height = 2.0f;
    public float teleportFadeTime = 2.0f;
    public GameObject instructions;
    public MovementHand movementController = MovementHand.Right;
    public MovementDevice movementPointer = MovementDevice.Controller;
    public bool otherThumbstickForHeight = true;
    public float accelerationFactor = 1.0f;
    public float frictionFactor = 1.0f;
    public float maximumVelocity = 0.05f;
    public float maximumFlyingHeight = 20.0f;
    public RotationStyle rotationStyle = RotationStyle.Stepped;
    public float steppingAngle = 30;
    public float rotationFrictionFactor = 0.5f;
    public float rotationAccelerationFactor = 2.0f;
    public string sceneSettingsObjectName = "ENTRY";
    public bool dynamicQuality = true;
    public SceneSettingsAntiAliasing movingAntiAliasingLevel = SceneSettingsAntiAliasing.None;
    public SceneSettingsTextureQuality movingTextureQuality = SceneSettingsTextureQuality.Eighth;
    public SceneSettingsVisualQuality movingVisualQuality = SceneSettingsVisualQuality.Medium;
    public ShadowQuality movingShadowQuality = ShadowQuality.Disable;
    public ShadowResolution movingShadowResolution = ShadowResolution.Low;
    public SceneSettingsAntiAliasing standingAntiAliasingLevel = SceneSettingsAntiAliasing.EightTimes;
    public SceneSettingsTextureQuality standingTextureQuality = SceneSettingsTextureQuality.Full;
    public SceneSettingsVisualQuality standingVisualQuality = SceneSettingsVisualQuality.High;
    public ShadowQuality standingShadowQuality = ShadowQuality.All;
    public ShadowResolution standingShadowResolution = ShadowResolution.VeryHigh;
    public GameObject leftMarker;
    public GameObject rightMarker;
    public GameObject leftPointer;
    public GameObject rightPointer;
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private XRType XRMode = XRType.Immersive_XR;
    private Vector3 velocity;
    private Vector3 acceleration;
    private float angularVelocity;
    private float angularAcceleration;
    private float angularStepTime;
    private float hitFrictionFactor = 0.0f; // Between 0.0f and 0.5f
    private bool movingToTarget = false;
    private bool fadingInAndOut = false;
    private float startFadeTime;
    private Renderer teleportFaderRenderer;
    private Vector3 targetDestination;
    private float startFlyingTime;
    private bool flying = false;
    private bool moved = false;
    private Vector3 markerOriginalSize;
    private bool isMovingTo = false;

    private bool currentlyHighQuality = false;

    private bool currentDynamicQuality;
    private SceneSettingsAntiAliasing currentMovingAntiAliasingLevel;
    private SceneSettingsTextureQuality currentMovingTextureQuality;
    private SceneSettingsVisualQuality currentMovingVisualQuality;
    private ShadowQuality currentMovingShadowQuality;
    private ShadowResolution currentMovingShadowResolution;
    private SceneSettingsAntiAliasing currentStandingAntiAliasingLevel;
    private SceneSettingsTextureQuality currentStandingTextureQuality;
    private SceneSettingsVisualQuality currentStandingVisualQuality;
    private ShadowQuality currentStandingShadowQuality;
    private ShadowResolution currentStandingShadowResolution;
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Awake()
    {
        markerOriginalSize = rightMarker.transform.localScale;
        XRMode = (XRSettings.isDeviceActive) ? XRType.Immersive_XR : XRType.Desktop_XR;      
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set up the link to the event manager
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        // Only use the height in desktop mode
        if (XRSettings.isDeviceActive) height = 0;

        // Set the body height;
        if (mainBody != null) mainBody.transform.localPosition = new Vector3(mainBody.transform.localPosition.x, height, mainBody.transform.localPosition.z);

        // Listen for events coming from the XR Controllers and other devices
        if (XRRig.EventQueue != null) XRRig.EventQueue.AddListener(OnDeviceEvent);

        // Add the function to call when a scene changes and set the default values
        SceneManager.sceneLoaded += OnSceneLoaded;
        GetEntrySettings();

        // Make sure the teleport fader is cleared away
        if (teleportFader != null)
        {
            teleportFaderRenderer = teleportFader.GetComponent<Renderer>();
            if (teleportFaderRenderer != null)
            {
                Material theMaterial = teleportFaderRenderer.material;
                theMaterial.SetColor("_Color", new Color(theMaterial.color.r, theMaterial.color.r, theMaterial.color.r, 0.0f));
            }
            teleportFader.SetActive(false);
        }

        // Make sure the instructions are visible
        if (instructions != null) instructions.SetActive(true);

        // Make sure the body and thePlayer start at 0
        StandOnGround();

        // Adjust some of the input parameters
        // accelerationFactor = Mathf.Clamp(accelerationFactor, 1.0f, 5.0f);

    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Functions to set parameters from UX elements
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void SetMovementStyle(XRData selection)
    {
        movementStyle = (selection.ToInt() == 0) ? MovementStyle.moveToMarker : MovementStyle.teleportToMarker;
    }
    public void SetRotationStyle(XRData selection)
    {
        rotationStyle = (selection.ToInt() == 0) ? RotationStyle.Stepped : RotationStyle.Smooth;
    }
    public void SetRotationAngle(XRData newAngle)
    {
        steppingAngle = newAngle.ToFloat();
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Slow down on external event
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void PutOnBrakes()
    {
        hitFrictionFactor = 5.0f;
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Checks downwards for an object that can be moved onto
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void StandOnGround()
    {
        if (thePlayer != null) thePlayer.transform.localPosition = Vector3.zero;

        RaycastHit hit;
        bool answer = (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), -Vector3.up, out hit, height, 1<<(int)OpenXR_UX_Layers.Go_Areas));
        if (answer)
        {
            transform.position = hit.point;
        }

        if (XRMode == XRType.Desktop_XR)
        {
            // if (theHead != null) theHead.transform.localPosition = new Vector3(0, 1.5f, 0);
            Camera.main.fieldOfView = 60.0f;
            rightMarker.SetActive(false);
        }
        else
        {
            Camera.main.fieldOfView = 26.99147f; // This number comes from the default setting for OpenXR
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Fade the view and move (to be called from the update function).
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void DoFadeAndMove()
    {
        // Calculate the amount of fade
        float t = (Time.time - startFadeTime) / teleportFadeTime;
        float newAlpha = 0.0f;
        if ((t >= 0.0f) && (t <= 0.5f))
        {
            // Fade to opaque
            newAlpha = Mathf.Lerp(0.0f, 1.0f, t * 2.0f);
        }
        else if ((t > 0.5f) && (t <= 1.0f))
        {
            // Fade to Transparent
            newAlpha = Mathf.Lerp(1.0f, 0.0f, (t - 0.5f) * 2.0f);                            
            // Move the view when faded out
            transform.position = targetDestination;
        }
        else
        {
            // Stop fading when faded in
            newAlpha = 0.0f;
            movingToTarget = fadingInAndOut = false;
            teleportFader.SetActive(false);
        }

        // Change the material color of the fader
        Material theMaterial = teleportFaderRenderer.material;
        theMaterial.SetColor("_Color", new Color(theMaterial.color.r, theMaterial.color.r, theMaterial.color.r, newAlpha));
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Check we're not crashing into no-go areas.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private bool ObstacleCheckDown(Vector3 position, Vector3 direction)
    {
        // Calculate the next step to take
        Vector3 nextStep = position + direction;

        // Take into account the person wearing the HMD's head height
        float headHeight = (theHead == null) ? height : theHead.transform.position.y - transform.position.y;

        RaycastHit hit_nogo;
        RaycastHit hit_go;
        bool nogo_down = Physics.Raycast(new Vector3(nextStep.x, nextStep.y + headHeight, nextStep.z), -Vector3.up, out hit_nogo, headHeight + 0.1f, 1<<(int)OpenXR_UX_Layers.NoGo_Areas);
        bool go_down = Physics.Raycast(new Vector3(nextStep.x, nextStep.y + headHeight, nextStep.z), -Vector3.up, out hit_go, headHeight + 0.1f, 1<<(int)OpenXR_UX_Layers.Go_Areas);

        bool answer = false;
        if (nogo_down && go_down)
        {
            // Obstacle if the nogo layer is above the go layer
            answer = hit_nogo.point.y > hit_go.point.y;            
        }
        else if (nogo_down)
        {
            answer = true;
        }

        // Raycast down from where we are going to be
        return (answer);
    }
    private bool ObstacleCheckForward(Vector3 position, Vector3 direction)
    {
        // Take into account the person wearing the HMD's headheight
        float headHeight = (theHead == null) ? height : theHead.transform.position.y - transform.position.y;

        // Raycast forward from current position towards next position
        return (Physics.Raycast(
            new Vector3(position.x, position.y + headHeight, position.z), 
            new Vector3(direction.x, 0.0f, direction.z), 
            Vector3.Magnitude(direction) * 10.0f, 1<<(int)OpenXR_UX_Layers.NoGo_Areas));
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Find distance above areas
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private bool HeightAbove(Vector3 position, out float newHeight)
    {
        RaycastHit hit;
        float headHeight = (theHead == null) ? height : theHead.transform.position.y - transform.position.y;
        bool answer =  (Physics.Raycast(new Vector3(position.x, position.y + headHeight, position.z), -Vector3.up, out hit, headHeight + 0.5f, 1<<(int)OpenXR_UX_Layers.Go_Areas));
        if (answer)
        {
            newHeight = hit.point.y - position.y;
        }
        else
        {
            newHeight = 0.0f;
        }
        return (answer);
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Make the movements and rotations
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        Vector3 newPosition;

        // If moving to the target
        if (movingToTarget)
        {
            if (movementStyle == MovementStyle.teleportToMarker)
            {
                // Teleporting, so fade in and out, and move
                if (teleportFader != null)
                {
                    if (fadingInAndOut) DoFadeAndMove();
                }
                else
                {
                    // No fader, so just move there.
                    transform.position = targetDestination;
                    movingToTarget = fadingInAndOut = false;
                }

                // Stop any other movement
                acceleration = Vector3.zero;
                velocity = Vector3.zero;
                hitFrictionFactor = 0.0f;
            }
            else
            {
                // Sliding along to the target
                // Essentially a spring equation...
                acceleration = (targetDestination - transform.position) * Time.deltaTime;

                // Decelerate as we near the target (ie damping of the spring) so we don't overshoot and bounce back
                hitFrictionFactor = 20.0f / (1.0f + Vector3.Distance(transform.position, targetDestination));

                // Stop moving if at the target
                if (transform.position == targetDestination)
                {
                    movingToTarget = false;
                    acceleration = Vector3.zero;
                    velocity = Vector3.zero;
                    hitFrictionFactor = 0.0f;
                }
            }
        }

        // Work out new velocity taking into account acceleration and friction
        // Acceleration comes from the user controls or from moving to the target
        // Friction is based on Velocity (ie Kinetic Friction)
        Vector3 deltaVelocity = acceleration * accelerationFactor * Time.deltaTime;

        // Friction is used to slow the movement once no controls are being pushed.  Friction is a function of velocity.
        Vector3 friction = new Vector3 (
            velocity.x * (0.5f + hitFrictionFactor) * frictionFactor * Time.deltaTime,
            velocity.y * (0.5f + hitFrictionFactor) * frictionFactor * Time.deltaTime,  
            velocity.z * (0.5f + hitFrictionFactor) * frictionFactor * Time.deltaTime
        );

        // The velocity change is the previous velocity + the change through acceleration - friction
        velocity = velocity + deltaVelocity - friction;

        // limit the velocity so we don't go shooting away really fast and make people nauseous
        velocity = Vector3.ClampMagnitude ( velocity, maximumVelocity );

        // Work out the potential new position
        newPosition = transform.position + velocity;

        // Restrict how high the user can fly
        newPosition.y = Mathf.Clamp(newPosition.y, 0.0f, maximumFlyingHeight);

        // Now we know where we want to move to, but have to make sure there are no obstacles in the way.  This is being done using raycasting rather
        // than collisions as it is simpler for mobile devices, but can sometimes result in odd situations whereby you can slide through objects that should
        // be solid.

        // Check if flying or not, and behave accordingly
        if (flying)
        {
            // Move there if there are no obstacles ahead, otherwise stop moving.
            if (ObstacleCheckForward(transform.position, velocity))
            {
                velocity = Vector3.zero;
                acceleration = Vector3.zero;
            }
            else
            {
                transform.position = newPosition;

                // Check if we are near the ground
                float groundHeight = 0.0f;
                bool hittingGround = HeightAbove(newPosition, out groundHeight);

                // Alow half a second to start flying, or if getting really close to a 'ground' object
                if ((Time.time - startFlyingTime) > 0.5f)
                {
                    // If after that time, we are still on the ground, then stay on the ground
                    if (hittingGround)
                    {
                        flying = false;
                    }
                }              
            }
        }
        else
        {
            // Move there if there are no obstacles, otherwise stop moving.
            if (ObstacleCheckDown(transform.position, velocity) || ObstacleCheckForward(transform.position, velocity))
            {
                velocity = Vector3.zero;
                acceleration = Vector3.zero;
            }
            else
            {
                // Do the movement
                float groundHeight = 0.0f;
                if (HeightAbove(newPosition, out groundHeight))
                {
                    transform.position = new Vector3(newPosition.x, newPosition.y + groundHeight, newPosition.z);
                }
                else
                {
                    transform.position = newPosition;
                }                 
            }       
        }

        // Adjust Quality settings on movement
        if (currentDynamicQuality)
        {
            if ((velocity.magnitude <= 0.005f) && (Mathf.Abs(angularVelocity) <= 0.05f))
            {
                if (!currentlyHighQuality)
                {
                    switch (currentStandingAntiAliasingLevel)
                    {
                        case SceneSettingsAntiAliasing.None: QualitySettings.antiAliasing = 0; break;
                        case SceneSettingsAntiAliasing.TwoTimes: QualitySettings.antiAliasing = 2; break;
                        case SceneSettingsAntiAliasing.EightTimes: QualitySettings.antiAliasing = 8; break;
                        default: QualitySettings.antiAliasing = 4; break;
                    }
                    QualitySettings.masterTextureLimit = 3 - (int) currentStandingTextureQuality;
                    XRSettings.eyeTextureResolutionScale = (int) currentStandingVisualQuality / 4.0f + 0.5f;
                    QualitySettings.shadows = currentStandingShadowQuality;
                    QualitySettings.shadowResolution = currentStandingShadowResolution;
                    currentlyHighQuality = true;
                }
            }
            else
            {
                if (currentlyHighQuality)
                {
                    switch (currentMovingAntiAliasingLevel)
                    {
                        case SceneSettingsAntiAliasing.None: QualitySettings.antiAliasing = 0; break;
                        case SceneSettingsAntiAliasing.TwoTimes: QualitySettings.antiAliasing = 2; break;
                        case SceneSettingsAntiAliasing.EightTimes: QualitySettings.antiAliasing = 8; break;
                        default: QualitySettings.antiAliasing = 4; break;
                    }
                    QualitySettings.masterTextureLimit = 3 - (int) currentMovingTextureQuality;
                    XRSettings.eyeTextureResolutionScale = (int) currentMovingVisualQuality / 4.0f + 0.5f;
                    QualitySettings.shadows = currentMovingShadowQuality;
                    QualitySettings.shadowResolution = currentMovingShadowResolution;
                    currentlyHighQuality = false;
                }
            }
        }

        // Work out rotation
        angularVelocity = angularVelocity + (angularAcceleration * Time.deltaTime * rotationAccelerationFactor) + (angularVelocity * Time.deltaTime * -rotationFrictionFactor);
        if (rotationStyle == RotationStyle.Stepped)
        {
            if ((Time.time - angularStepTime) > 0.5f)
            {
                if (angularAcceleration > 0.001)
                {
                    transform.eulerAngles = transform.eulerAngles + new Vector3(0, steppingAngle, 0);
                    angularStepTime = Time.time;
                }
                else if (angularAcceleration < -0.001)
                {
                    transform.eulerAngles = transform.eulerAngles + new Vector3(0, -steppingAngle, 0);
                    angularStepTime = Time.time;
                }
                else
                {
                    // Make is so that the next rotation happens immediately the button is pressed.
                    angularStepTime = Time.time - 0.5f;
                }
            }
        }
        else
        {
            transform.Rotate(0.0f, angularVelocity, 0.0f);
        }

        // Set head movements and track a marker with the mouse in Desktop mode
        if (XRMode == XRType.Desktop_XR)
        {
            RaycastHit hit;

            // Head Movement
            float xpos = Mathf.Clamp((Input.mousePosition.x - Screen.width / 2.0f) / (Screen.width / 2.0f), -1.0f, 1.0f);
            float ypos = Mathf.Clamp((Input.mousePosition.y - Screen.height / 2.0f) / (Screen.height / 2.0f), -1.0f, 1.0f);

            theHead.transform.localRotation = Quaternion.Euler(ypos * -30.0f, xpos * 45.0f, 0.0f);

            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100, 1<<(int)OpenXR_UX_Layers.OpenXR_UX | 1<<(int)OpenXR_UX_Layers.Go_Areas | 1<<(int)OpenXR_UX_Layers.NoGo_Areas))
            {
                int theLayer = hit.collider.gameObject.layer;
                switch (theLayer)
                {
                    case (int)OpenXR_UX_Layers.OpenXR_UX:
                        rightMarker.transform.position = hit.point;
                        rightMarker.transform.localScale = markerOriginalSize;
                        rightMarker.SetActive(true);
                        isMovingTo = false;
                        // trail.enabled = true;
                        // trail.startWidth = 0.005f;
                        // trail.endWidth = 0.001f;
                        // for (int i = 0; i < trailDensity; i++)
                        // {
                        //     trailPoints[i] = TravelCurve(transform.position, Marker.transform.position, 0.0f, ((i * 1.0f) / (trailDensity  * 1.0f)), Vector3.up);
                        // }
                        // trail.SetPositions(trailPoints);
                        break;
                    case (int)OpenXR_UX_Layers.Go_Areas:
                        rightMarker.transform.position = hit.point;
                        rightMarker.transform.localScale = markerOriginalSize * 30.0f;
                        isMovingTo = true;
                        rightMarker.SetActive(true);
                        // trail.enabled = true;
                        // trail.startWidth = 0.002f;
                        // trail.endWidth = 0.1f;
                        // for (int i = 0; i < trailDensity; i++)
                        // {
                        //     trailPoints[i] = TravelCurve(transform.position, Marker.transform.position, 0.3f, (((i + 1) * 1.0f) / ((trailDensity + 2) * 1.0f)), Vector3.up);
                        // }
                        // trail.SetPositions(trailPoints);
                        break;
                    default:
                        rightMarker.transform.localPosition = Vector3.zero;
                        rightMarker.transform.localScale = markerOriginalSize;
                        rightMarker.SetActive(false);
                        isMovingTo = false;
                        // trail.enabled = false;
                        break;
                }
            }
            else
            {
                rightMarker.transform.localPosition = Vector3.zero;
                rightMarker.transform.localScale = markerOriginalSize;
                rightMarker.SetActive(false);                
                // trail.enabled = false;
            }
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Remove the instructions object from view
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void TestAndRemoveInstructions()
    {
        if (!moved)
        {
            moved = true;
            if (instructions != null) instructions.SetActive(false);
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Helper functions
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void TestMoveOrTeleport(XREvent theEvent, XRDeviceEventTypes eventType, GameObject marker, GameObject pointer)
    {
        if ((theEvent.eventType == eventType) && (theEvent.eventAction == XRDeviceActions.CLICK))
        {
            // Remove Instructions on first move
            TestAndRemoveInstructions();

            if (theEvent.eventBool)
            {
                if (XRMode == XRType.Desktop_XR)
                {
                    if (isMovingTo)
                    {
                        movingToTarget = true;
                        targetDestination = rightMarker.transform.position;
                        if (movementStyle == MovementStyle.teleportToMarker)
                        {
                            if (teleportFader != null) teleportFader.SetActive(true);
                            startFadeTime = Time.time;
                            fadingInAndOut = true;
                        }
                    }
                    else movingToTarget = false;                    
                }
                else
                {
                    if ((marker != null) && (pointer != null))
                    {
                        XRRig_Pointer pointerScript = pointer.GetComponent<XRRig_Pointer>();
                        if (pointerScript != null)
                        {
                            if (pointerScript.IsMovingTo)
                            {
                                movingToTarget = true;
                                targetDestination = marker.transform.position;
                                if (movementStyle == MovementStyle.teleportToMarker)
                                {
                                    if (teleportFader != null) teleportFader.SetActive(true);
                                    startFadeTime = Time.time;
                                    fadingInAndOut = true;
                                }
                            }
                            else movingToTarget = false;
                        }
                        else movingToTarget = false;
                    }
                    else movingToTarget = false;
                }
            }
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Move as directed.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void OnDeviceEvent(XREvent theEvent)
    {
        // Move to the Target
        TestMoveOrTeleport(theEvent, XRDeviceEventTypes.left_grip, leftMarker, leftPointer);
        TestMoveOrTeleport(theEvent, XRDeviceEventTypes.right_grip, rightMarker, rightPointer);

        // Thumbstick
        if ((((theEvent.eventType == XRDeviceEventTypes.right_thumbstick) && (theEvent.eventAction == XRDeviceActions.MOVE)) && (movementController == MovementHand.Right)) ||
        (((theEvent.eventType == XRDeviceEventTypes.left_thumbstick) && (theEvent.eventAction == XRDeviceActions.MOVE)) && (movementController == MovementHand.Left)))
        {
            // Remove Instructions on first move
            TestAndRemoveInstructions();

            Vector2 thumbstickMovement = theEvent.eventVector;

            // Turn left or right
            if (Mathf.Abs(thumbstickMovement.x) > 0.5f)
            {
                angularAcceleration = Mathf.Sign(thumbstickMovement.x) * 0.1f;
            }
            else
            {
                angularAcceleration = 0.0f;
            }

            // Move forward or back
            if (Mathf.Abs(thumbstickMovement.y) > 0.5f)
            {
                if (movementPointer == MovementDevice.Head)
                {
                    // Move in the direction of the head (ie the main camera)
                    acceleration = Camera.main.gameObject.transform.forward * Mathf.Sign(thumbstickMovement.y) * 0.01f;
                }
                else
                {
                    // Move in the direction of the selected controller
                    if (movementController == MovementHand.Right) 
                        acceleration = rightPointer.transform.forward * Mathf.Sign(thumbstickMovement.y) * 0.01f;
                    else
                        acceleration = leftPointer.transform.forward * Mathf.Sign(thumbstickMovement.y) * 0.01f;
                }
                
                // Stop any movement towards the target marker and remove residual friction
                hitFrictionFactor = 0.0f;
                if (movingToTarget)
                {
                    if (teleportFader != null) teleportFader.SetActive(false);
                    movingToTarget = false;
                }
            }
            else
            {
                // Stop pushing
                acceleration = Vector3.zero;
            }
        }

        // Height controlled by other thumbstick
        if ( otherThumbstickForHeight )
        {
            if ((((theEvent.eventType == XRDeviceEventTypes.right_thumbstick) && (theEvent.eventAction == XRDeviceActions.MOVE)) && (movementController == MovementHand.Left)) ||
            (((theEvent.eventType == XRDeviceEventTypes.left_thumbstick) && (theEvent.eventAction == XRDeviceActions.MOVE)) && (movementController == MovementHand.Right)))
            {
                // Remove Instructions on first move
                TestAndRemoveInstructions();

                Vector2 thumbstickMovement = theEvent.eventVector;
        
                // Move up or down
                if (Mathf.Abs(thumbstickMovement.y) > 0.5f)
                {
                    acceleration = new Vector3(acceleration.x, Mathf.Sign(thumbstickMovement.y) * 0.01f, acceleration.z);
                }
                else
                {
                    acceleration = new Vector3(acceleration.x, 0.0f, acceleration.z);
                }

                // if (thumbstickMovement.y > 0.5)
                if ((acceleration.y > 0.0f) && !flying)
                {
                    flying = true;
                    startFlyingTime = Time.time;
                    hitFrictionFactor = 0.0f;
                    if (movingToTarget)
                    {
                        if (teleportFader != null) teleportFader.SetActive(false);
                        movingToTarget = false;
                    }
                }
            }
            else
            {
                // Only move up and down when thumbstick being used.
                acceleration = new Vector3(acceleration.x, 0.0f, acceleration.z);
            }
        }
        else
        {
            if ((acceleration.y > 0.0f) && !flying)
            {
                flying = true;
                startFlyingTime = Time.time;
                hitFrictionFactor = 0.0f;
                if (movingToTarget)
                {
                    if (teleportFader != null) teleportFader.SetActive(false);
                    movingToTarget = false;
                }
            }
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // When the Scene is loaded, check if there are scene settings to alter.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void SetSceneSettingsToDefaults()
    {
        currentDynamicQuality = dynamicQuality;
        currentMovingAntiAliasingLevel = movingAntiAliasingLevel;
        currentMovingTextureQuality = movingTextureQuality;
        currentMovingVisualQuality = movingVisualQuality;
        currentMovingShadowQuality = movingShadowQuality;
        currentMovingShadowResolution = movingShadowResolution;
        currentStandingAntiAliasingLevel = standingAntiAliasingLevel;
        currentStandingTextureQuality = standingTextureQuality;
        currentStandingVisualQuality = standingVisualQuality;
        currentStandingShadowQuality = standingShadowQuality;
        currentStandingShadowResolution = standingShadowResolution;          
    }
    private void OnSceneLoaded (Scene scene, LoadSceneMode mode)
    {
        GetEntrySettings();
    }
    private void GetEntrySettings()
    {
        // Find the entry point if it exists
        GameObject ENTRY = GameObject.Find(sceneSettingsObjectName);
        if (ENTRY == null)
        {
            SetSceneSettingsToDefaults();              
        }
        else
        {
            XRUX_SceneSettings sceneSettings = ENTRY.GetComponent<XRUX_SceneSettings>();
            if (sceneSettings != null)
            {
                currentDynamicQuality = sceneSettings.dynamicQuality;
                currentMovingAntiAliasingLevel = sceneSettings.movingAntiAliasingLevel;
                currentMovingTextureQuality = sceneSettings.movingTextureQuality;
                currentMovingVisualQuality = sceneSettings.movingVisualQuality;
                currentMovingShadowQuality = sceneSettings.movingShadowQuality;
                currentMovingShadowResolution = sceneSettings.movingShadowResolution;
                currentStandingAntiAliasingLevel = sceneSettings.standingAntiAliasingLevel;
                currentStandingTextureQuality = sceneSettings.standingTextureQuality;
                currentStandingVisualQuality = sceneSettings.standingVisualQuality;
                currentStandingShadowQuality = sceneSettings.standingShadowQuality;
                currentStandingShadowResolution = sceneSettings.standingShadowResolution;
            }
            else
            {
                SetSceneSettingsToDefaults();              
            }
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
