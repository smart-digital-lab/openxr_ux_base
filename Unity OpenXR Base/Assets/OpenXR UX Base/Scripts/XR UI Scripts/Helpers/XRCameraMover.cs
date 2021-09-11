/**********************************************************************************************************************************************************
 * XRCameraMover
 * -------------
 *
 * 2021-09-07
 *
 * Moves the camera under program (ie user) control.  Should be placed on the XRRig.
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
public interface _XRCameraMover
{
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------

[Serializable]
public enum MovementStyle { teleportToMarker, moveToMarker }
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public class XRCameraMover : MonoBehaviour, _XRCameraMover
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Some states for helping with the movements and rotations
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    enum MovementStates { stopped, posAccel, posDecel, negAccel, negDecel, moveTo }
    enum TurningStates { stopped, leftAccel, rightAccel, leftDecel, rightDecel }
    enum HeightStates { stopped, upAccel, downAccel, upDecel, downDecel }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    [Header("____________________________________________________________________________________________________")]
    [Header("Move the viewer's camera in a smooth and intuitive manner.\n____________________________________________________________________________________________________")]
    [Header("INPUTS\n\n - Data from the openXR Controllers.")]

    [Header("____________________________________________________________________________________________________")]
    [Header("SETTINGS")]
    [Header("Movement Style")]
    public MovementStyle movementStyle = MovementStyle.teleportToMarker;
    [Header("The marker objects.")]
    public GameObject leftMarker;
    public GameObject rightMarker;
    [Header("Maximum camera rotation speed (degrees per second).")]
    public float rotationSpeed = 30.0f;
    [Header("Maximum camera movement speed (units per second).")]
    public float movingSpeed = 10.0f;
    [Header("Time to reach full rotation speed (seconds).")]
    public float rotationTime = 2.0f;
    [Header("Time to reach full movement speed (seconds).")]
    public float movementTime = 1.0f;
    [Header("Time to move to marker (seconds).")]
    public float markerTime = 5.0f;
    [Header("Ascent and descent speed (units per second).")]
    public float heightSpeed = 1.0f;
    
    [Header("____________________________________________________________________________________________________")]
    [Header("OUTPUTS\n\n - Viewpoint movement")]
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private XRDeviceManager watcher; // The XR Event Manager
    private TurningStates turning = TurningStates.stopped;
    private MovementStates movementX = MovementStates.stopped;
    private MovementStates movementY = MovementStates.stopped;
    private HeightStates headHeight = HeightStates.stopped;

    private float startRotTime, startMoveXTime, startMoveYTime, startHeightTime;
    private Vector3 startPosition;
    private Vector3 newPosition;
    private Vector3 velocity = Vector3.zero;
    private UnityEditor.XR.LegacyInputHelpers.CameraOffset cameraOffset;
    private float currentHeightSpeed, currentRotationSpeed, currentMoveXSpeed, currentMoveYSpeed = 0.0f;
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
        cameraOffset = GetComponent<UnityEditor.XR.LegacyInputHelpers.CameraOffset>();
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // What to do when we collide with stuff
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void startCollided(Collider other)
    {
        movementY = movementX = MovementStates.stopped;
    }
    public void stillCollided(Collider other)
    {
        //movementY = movementX = MovementStates.stopped;
    }
    public void stopCollided(Collider other)
    {

    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Service functions for calculations below
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private float CalculateAcceleration(float startTime, float acceleration, float lower, float upper)
    {
        float t = (Time.time - startTime) / acceleration;
        return (Mathf.SmoothStep(lower, upper, t));
    }
    private Vector3 CreateMovementVector(Vector3 direction, float speed)
    {
        Vector3 newPosition = transform.position + direction * speed / 100.0f;
        return( new Vector3(newPosition.x, 0, newPosition.z) );
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // 
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        switch (turning)
        {
            // Accelerate turning left or right
            case TurningStates.leftAccel:
            case TurningStates.rightAccel:
                currentRotationSpeed = CalculateAcceleration(startRotTime, rotationTime, 0.0f, rotationSpeed);
                transform.Rotate(0, ((turning == TurningStates.leftAccel) ? -1 : 1) * currentRotationSpeed * Time.deltaTime, 0);
                break;

            // Decelerate turning left or right
            case TurningStates.leftDecel:
            case TurningStates.rightDecel:
                float rotationDecel = CalculateAcceleration(startRotTime, rotationTime, currentRotationSpeed, 0.0f);
                transform.Rotate(0, ((turning == TurningStates.leftDecel) ? -1 : 1) * rotationDecel * Time.deltaTime, 0);
                turning = (Mathf.Abs(rotationDecel) <= 0.001f) ? TurningStates.stopped : turning;
                break;

            default:
                break;
        }

        switch (movementY)
        {
            // Moving to the marker
            case MovementStates.moveTo:
                if (movementStyle == MovementStyle.teleportToMarker)
                {
                    transform.position = newPosition;
                    movementY = MovementStates.stopped;
                }
                else
                {
                    // Move smoothly towards the destination
                    transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, markerTime);
                    // Stop when we get there
                    if (transform.position == newPosition) movementX = MovementStates.stopped;
                }
                break;

            // Accelerate moving forward and back
            case MovementStates.posAccel:
            case MovementStates.negAccel:
                currentMoveYSpeed = CalculateAcceleration(startMoveYTime, movementTime, 0.0f, movingSpeed);
                transform.position = CreateMovementVector(((movementY == MovementStates.negAccel) ? -1 : 1) * Camera.main.transform.forward, currentMoveYSpeed);
                break;

            // Decelerate moving forward and back
            case MovementStates.posDecel:
            case MovementStates.negDecel:
                float forwardDecel = CalculateAcceleration(startMoveYTime, movementTime, currentMoveYSpeed, 0.0f);
                transform.position = CreateMovementVector(((movementY == MovementStates.negDecel) ? -1 : 1) * Camera.main.transform.forward, forwardDecel);
                movementY = (Mathf.Abs(forwardDecel) <= 0.001f) ? MovementStates.stopped : movementY;
                break;

            default:
                break;
        }

        switch (movementX)
        {
            // Accelerate moving left or right
            case MovementStates.posAccel:
            case MovementStates.negAccel:
                currentMoveXSpeed = CalculateAcceleration(startMoveXTime, movementTime, 0.0f, movingSpeed);
                transform.position = CreateMovementVector(((movementX == MovementStates.negAccel) ? -1 : 1) * Camera.main.transform.right, currentMoveXSpeed);
                break;

            // Decelerate moving left or right
            case MovementStates.posDecel:
            case MovementStates.negDecel:
                float sidemoveDecel = CalculateAcceleration(startMoveXTime, movementTime, currentMoveXSpeed, 0.0f);
                transform.position = CreateMovementVector(((movementX == MovementStates.negDecel) ? -1 : 1) * Camera.main.transform.right, sidemoveDecel);
                movementX = (Mathf.Abs(sidemoveDecel) <= 0.001f) ? MovementStates.stopped : movementX;
                break;

            default:
                break;
        }

        switch (headHeight)
        {
            case HeightStates.upAccel:
                currentHeightSpeed = CalculateAcceleration(startHeightTime, 1.0f, 0.0f, heightSpeed);
                cameraOffset.cameraYOffset = Mathf.Min(10.0f, cameraOffset.cameraYOffset + currentHeightSpeed / 100.0f);
                break;
            case HeightStates.downAccel:
                currentHeightSpeed = CalculateAcceleration(startHeightTime, 1.0f, 0.0f, heightSpeed);
                cameraOffset.cameraYOffset = Mathf.Max(0.5f, cameraOffset.cameraYOffset - currentHeightSpeed / 100.0f);
                break;
            case HeightStates.upDecel:
                float upDecel = CalculateAcceleration(startHeightTime, 1.0f, currentHeightSpeed, 0.0f);
                cameraOffset.cameraYOffset = Mathf.Min(10.0f, cameraOffset.cameraYOffset + upDecel / 100.0f);
                headHeight = (Mathf.Abs(upDecel) <= 0.001f) ? HeightStates.stopped : headHeight;
                break;
            case HeightStates.downDecel:
                float downDecel = CalculateAcceleration(startHeightTime, 1.0f, currentHeightSpeed, 0.0f);
                cameraOffset.cameraYOffset = Mathf.Max(0.5f, cameraOffset.cameraYOffset - downDecel / 100.0f);
                headHeight = (Mathf.Abs(downDecel) <= 0.001f) ? HeightStates.stopped : headHeight;
                break;
            default:
                break;
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Move as directed.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void onDeviceEvent(XREvent theEvent)
    {
        // Move to the Target
        if ((theEvent.eventType == XRDeviceEventTypes.left_grip) && (theEvent.eventAction == XRDeviceActions.CLICK))
        {
            if (theEvent.eventBool)
            {
                if (leftMarker != null)
                {
                    startPosition = transform.position;
                    newPosition = new Vector3(leftMarker.transform.position.x, 0, leftMarker.transform.position.z);
                    movementY = MovementStates.moveTo;
                }
            }
        }
        else if ((theEvent.eventType == XRDeviceEventTypes.right_grip) && (theEvent.eventAction == XRDeviceActions.CLICK))
        {
            if (theEvent.eventBool)
            {
                if (rightMarker != null)
                {
                    startPosition = transform.position;
                    newPosition = new Vector3(rightMarker.transform.position.x, 0, rightMarker.transform.position.z);
                    movementY = MovementStates.moveTo;
                }
            }
        }

        // Right Thumbstick
        if ((theEvent.eventType == XRDeviceEventTypes.right_thumbstick) && (theEvent.eventAction == XRDeviceActions.MOVE))
        {
            Vector2 thumbstickMovement = theEvent.eventVector;

            // Turn left or right
            if (thumbstickMovement.x < -0.5)
            {
                if (turning != TurningStates.leftAccel) startRotTime = Time.time;
                turning = TurningStates.leftAccel;
            }
            else if (thumbstickMovement.x > 0.5)
            {
                if (turning != TurningStates.rightAccel) startRotTime = Time.time;
                turning = TurningStates.rightAccel;
            }
            else
            {
                if (turning == TurningStates.leftAccel) { turning = TurningStates.leftDecel; startRotTime = Time.time; }
                else if (turning == TurningStates.rightAccel) { turning = TurningStates.rightDecel; startRotTime = Time.time; }
            }

            // Move forward or back
            if (thumbstickMovement.y < -0.5)
            {
                if (movementY != MovementStates.negAccel) { startMoveYTime = Time.time; startPosition = transform.position; }
                movementY = MovementStates.negAccel;
            }
            else if (thumbstickMovement.y > 0.5)
            {
                if (movementY != MovementStates.posAccel) { startMoveYTime = Time.time; startPosition = transform.position; }
                movementY = MovementStates.posAccel;
            }
            else
            {
                if (movementY == MovementStates.posAccel) { movementY = MovementStates.posDecel; startMoveYTime = Time.time; }
                else if (movementY == MovementStates.negAccel) { movementY = MovementStates.negDecel; startMoveYTime = Time.time; }
            }
        }

        // Left Thumbstick
        if ((theEvent.eventType == XRDeviceEventTypes.left_thumbstick) && (theEvent.eventAction == XRDeviceActions.MOVE))
        {
            Vector2 thumbstickMovement = theEvent.eventVector;

            // Strafe left or right
            if (thumbstickMovement.x < -0.5)
            {
                if (movementX != MovementStates.negAccel) startMoveXTime = Time.time;
                movementX = MovementStates.negAccel;
            }
            else if (thumbstickMovement.x > 0.5)
            {
                if (movementX != MovementStates.posAccel) startMoveXTime = Time.time;
                movementX = MovementStates.posAccel;
            }
            else
            {
                if (movementX == MovementStates.posAccel) { movementX = MovementStates.posDecel; startMoveXTime = Time.time; }
                else if (movementX == MovementStates.negAccel) { movementX = MovementStates.negDecel; startMoveXTime = Time.time; }
            }

            // Move camera up or down
            if (cameraOffset != null)
            {
                if (thumbstickMovement.y > 0.5)
                {
                    if (headHeight != HeightStates.upAccel) startHeightTime = Time.time;
                    headHeight = HeightStates.upAccel;
                }
                else if (thumbstickMovement.y < -0.5)
                {
                    if (headHeight != HeightStates.downAccel) startHeightTime = Time.time;
                    headHeight = HeightStates.downAccel;
                }
                else
                {
                    if (headHeight == HeightStates.upAccel) { headHeight = HeightStates.upDecel; startHeightTime = Time.time; }
                    else if (headHeight == HeightStates.downAccel) { headHeight = HeightStates.downDecel; startHeightTime = Time.time; }
                }
            }           
        }

    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
