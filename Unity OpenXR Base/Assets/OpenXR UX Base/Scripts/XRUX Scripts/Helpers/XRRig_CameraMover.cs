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

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Public functions
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public interface _XRRig_CameraMover
{
    void PutOnBrakes();                         // Slow down all movement
    void SetMovementStyle(XRData selection);    // Set the movement style (0 = teleport, 1 = move)
    void StandOnGround();                       // Move the viewpoint to standing on the ground
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public class XRRig_CameraMover : MonoBehaviour, _XRRig_CameraMover
{
    public enum MovementStyle   { teleportToMarker, moveToMarker }
    public enum MovementHand    { Left, Right }
    public enum MovementDevice  { Head, Controller }
    public enum AntiAliasing    { None, TwoTimes, FourTimes, EightTimes}
    public enum TextureQuality  { Full, Half, Quarter, Eighth }

    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    [Header("____________________________________________________________________________________________________")]
    [Header("Move the viewer's camera in a smooth and intuitive manner.\n____________________________________________________________________________________________________")]
    [Header("INPUTS\n\n - Data from the openXR Controllers.")]

    [Header("____________________________________________________________________________________________________")]
    [Header("SETTINGS")]
    [Header("Teleport Style")]
    public MovementStyle movementStyle = MovementStyle.teleportToMarker;
    public GameObject teleportFader;
    public GameObject theHead;
    public GameObject thePlayer;
    public float teleportFadeTime = 2.0f;
    [Header("Instructions")]
    public GameObject instructions;
    [Header("Movement Control")]
    public MovementHand movementController = MovementHand.Right;
    public MovementDevice movementPointer = MovementDevice.Controller;
    public bool otherThumbstickForHeight = true;
    [Header("Movement Parameters")]
    public float accelerationFactor = 1.0f;
    public float maximumFlyingHeight = 20.0f;
    [Header("Rotation Parameters")]
    public float rotationFrictionFactor = 0.5f;
    public float rotationAccelerationFactor = 2.0f;
    [Header("Dynamic Quality Settings")]
    public bool dynamicQuality = true;
    [Header("When moving")]
    public AntiAliasing movingAntiAliasingLevel = AntiAliasing.None;
    public TextureQuality movingTextureQuality = TextureQuality.Eighth;
    [Header("When standing still")]
    public AntiAliasing standingAntiAliasingLevel = AntiAliasing.EightTimes;
    public TextureQuality standingTextureQuality = TextureQuality.Full;
    [Header("The marker and pointer objects")]
    public GameObject leftMarker;
    public GameObject rightMarker;
    public GameObject leftPointer;
    public GameObject rightPointer;
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private Vector3 velocity;
    private Vector3 acceleration;
    private float angularVelocity;
    private float angularAcceleration;
    private float hitFrictionFactor = 0.0f; // Between 0.0f and 0.5f
    //private float prevHitFrictionTime = 0.0f;

    private bool movingToTarget = false;
    private bool fadingInAndOut = false;
    private float startFadeTime;
    private Renderer teleportFaderRenderer;
    private Vector3 targetDestination;
    private float startFlyingTime;
    private bool flying = false;
    private bool moved = false;
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set up the link to the event manager
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        // Listen for events coming from the XR Controllers and other devices
        if (XRRig.EventQueue != null) XRRig.EventQueue.AddListener(OnDeviceEvent);

        // Start at the origin
        //transform.position = Vector3.zero;

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
        accelerationFactor = Mathf.Clamp(accelerationFactor, 1.0f, 5.0f);

    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Functions to set parameters from UX elements
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void SetMovementStyle(XRData selection)
    {
        movementStyle = (selection.ToInt() == 0) ? MovementStyle.moveToMarker : MovementStyle.teleportToMarker;
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
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void StandOnGround()
    {
        if (thePlayer != null) thePlayer.transform.localPosition = Vector3.zero;

        RaycastHit hit;
        bool answer = (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), -Vector3.up, out hit, 2.0f, 1<<7));
        if (answer)
        {
            transform.position = hit.point;
        }
        // else
        // {
        //     transform.position = Vector3.zero;         
        // }   
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Fade the view and move
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void DoFadeAndMove()
    {
        // Calculate the amount of fade
        float t = (Time.time - startFadeTime) / teleportFadeTime;
        float newAlpha = 0.0f;
        if ((t > 0.0f) && (t <= 0.5f))
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
        float headHeight = (theHead == null) ? 2.0f : theHead.transform.position.y - transform.position.y;

        // Raycast down from where we are going to be
        return (Physics.Raycast(new Vector3(nextStep.x, nextStep.y + headHeight, nextStep.z), -Vector3.up, headHeight + 0.1f, 1<<8));
    }
    private bool ObstacleCheckForward(Vector3 position, Vector3 direction)
    {
        // Take into account the person wearing the HMD's headheight
        float headHeight = (theHead == null) ? 2.0f : theHead.transform.position.y - transform.position.y;

        // Raycast forward from current position towards next position
        return (Physics.Raycast(
            new Vector3(position.x, position.y + headHeight, position.z), 
            new Vector3(direction.x, 0.0f, direction.z), 
            Vector3.Magnitude(direction) * 10.0f, 1<<8));
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Find distance above areas
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private bool heightAbove(Vector3 position, out float height)
    {
        RaycastHit hit;
        float headHeight = (theHead == null) ? 2.0f : theHead.transform.position.y - transform.position.y;
        bool answer =  (Physics.Raycast(new Vector3(position.x, position.y + headHeight, position.z), -Vector3.up, out hit, headHeight + 0.5f, 1<<7));
        if (answer)
        {
            height = hit.point.y - position.y;
        }
        else
        {
            height = 0.0f;
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
                acceleration = (targetDestination - transform.position) * 0.1f;

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

        // Friction is used to slow the movement once no controls are being pushed
        Vector3 friction = new Vector3(
            velocity.x * (0.5f + hitFrictionFactor) * Time.deltaTime,
            velocity.y * (0.5f + hitFrictionFactor / 2.0f) * Time.deltaTime,  // Less friction in the vertical
            velocity.z * (0.5f + hitFrictionFactor) * Time.deltaTime
        );

        // The velocity change is the previous velocity + the change through acceleration - friction
        velocity = velocity + deltaVelocity - friction;

        // limit the velocity so we don't go shooting away really fast and make people nauseous
        velocity = Vector3.ClampMagnitude ( velocity, 0.05f );

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
                bool hittingGround = heightAbove(newPosition, out groundHeight);

                // Alow a second to start flying, or if getting really close to a 'ground' object
                if ((Time.time - startFlyingTime) > 1.0f)
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
            // Check if we're are getting close to a no-go area
            // if (ObstacleCheckDown(transform.position, velocity * 10.0f) || ObstacleCheckForward(transform.position, velocity * 10.0f))
            // {
            //     hitFrictionFactor = 10.0f;
            //     acceleration = Vector3.zero;
            //     prevHitFrictionTime = Time.time;
            // }
            // else
            // {
            //     // Decelerate the friction slowly in case the object we hit is quite small and we end up checking positions past the object.
            //     float t = (Time.time - prevHitFrictionTime) / 2.0f; 
            //     hitFrictionFactor = Mathf.SmoothStep(10.0f, 0.0f, t);
            // }

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
                if (heightAbove(newPosition, out groundHeight))
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
        if (dynamicQuality)
        {
            if ((velocity.magnitude <= 0.005f) && (Mathf.Abs(angularVelocity) <= 0.01f))
            {
                if (!currentlyHighQuality)
                {
                    switch (standingAntiAliasingLevel)
                    {
                        case AntiAliasing.None: QualitySettings.antiAliasing = 0; break;
                        case AntiAliasing.TwoTimes: QualitySettings.antiAliasing = 2; break;
                        case AntiAliasing.EightTimes: QualitySettings.antiAliasing = 8; break;
                        default: QualitySettings.antiAliasing = 4; break;
                    }
                    QualitySettings.masterTextureLimit = (int) standingTextureQuality;
                    currentlyHighQuality = true;
                }
            }
            else
            {
                if (currentlyHighQuality)
                {
                    switch (movingAntiAliasingLevel)
                    {
                        case AntiAliasing.None: QualitySettings.antiAliasing = 0; break;
                        case AntiAliasing.TwoTimes: QualitySettings.antiAliasing = 2; break;
                        case AntiAliasing.EightTimes: QualitySettings.antiAliasing = 8; break;
                        default: QualitySettings.antiAliasing = 4; break;
                    }
                    QualitySettings.masterTextureLimit = (int) movingTextureQuality;
                    currentlyHighQuality = false;
                }
            }
        }

        // Work out rotation
        angularVelocity = angularVelocity + (angularAcceleration * Time.deltaTime * rotationAccelerationFactor) + (angularVelocity * Time.deltaTime * -rotationFrictionFactor);
        transform.Rotate(0.0f, angularVelocity, 0.0f);
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Remove the instructions object from view
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void TestAndRemoveInstructions()
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
    public void TestMoveOrTeleport(XREvent theEvent, XRDeviceEventTypes eventType, GameObject marker, GameObject pointer)
    {
        if ((theEvent.eventType == eventType) && (theEvent.eventAction == XRDeviceActions.CLICK))
        {
            // Remove Instructions on first move
            TestAndRemoveInstructions();

            if (theEvent.eventBool)
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
                    // Move in the direction of the head
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
                    acceleration = new Vector3(acceleration.x, Mathf.Sign(thumbstickMovement.y) * 0.005f, acceleration.z);
                }

                if (thumbstickMovement.y > 0.5)
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
            }
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
