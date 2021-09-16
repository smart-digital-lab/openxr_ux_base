/**********************************************************************************************************************************************************
 * XRPointer
 * ---------
 *
 * 2021-09-02
 *
 * Tracks a marker object into the 3D environment in the direction of the pointer.  Only hits objects on layer 6.  Must be placed on an object that is
 * moved around with the game controllers.  Note that GameObjects must have a collider and be onlayer 6 to have the marker rest on them.
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Public functions
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public interface _XRPointer
{
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public class XRPointer : MonoBehaviour, _XRPointer
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    [Header("____________________________________________________________________________________________________")]
    [Header("Detects when the user is pointing at clickable objects and places to move.\nShould go on the pointer object attached to the left and right Controllers.\n____________________________________________________________________________________________________")]
    [Header("INPUTS\n\n - From the XR Controllers")]

    [Header("____________________________________________________________________________________________________")]
    [Header("SETTINGS")]
    [Header("A GameObject in the SceneGraph to move to where the user is pointing.")]
    public GameObject Marker;
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private Vector3 markerOriginalSize;
    private bool isTouching = false;
    public bool IsTouching { get { return isTouching; } }
    private bool isMovingTo = false;
    public bool IsMovingTo { get { return isMovingTo; } }
// ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set up
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        if (Marker != null) 
        {
            markerOriginalSize = Marker.transform.localScale;      
            Marker.SetActive(false);
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Raycast from the pointer and move the marker if it hits objects on layer 6 (clickable objects) or layer 7 (objects to be able to move onto)
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        if (Marker != null)
        {
            Vector3 fwd = transform.TransformDirection(Vector3.forward);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, fwd, out hit, 100, 1<<6 | 1<<7 | 1<<8))
            {
                int theLayer = hit.collider.gameObject.layer;
                switch (theLayer)
                {
                    case 6:
                        Marker.transform.position = hit.point;
                        Marker.transform.localScale = markerOriginalSize;
                        Marker.SetActive(true);
                        isTouching = true;
                        isMovingTo = false;
                        break;
                    case 7:
                        Marker.transform.position = hit.point;
                        Marker.transform.localScale = markerOriginalSize * 20.0f;
                        isTouching = false;
                        isMovingTo = true;
                        Marker.SetActive(true);
                        break;
                    default:
                        Marker.transform.localPosition = Vector3.zero;
                        Marker.transform.localScale = markerOriginalSize;
                        Marker.SetActive(false);
                        isTouching = false;
                        isMovingTo = false;
                        break;
                }
            }
            else
            {
                Marker.transform.localPosition = Vector3.zero;
                Marker.transform.localScale = markerOriginalSize;
                Marker.SetActive(false);                
            }
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Make sure the marker is turned on or off when the pointer turns on or off
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void OnDisable()
    {
        if (Marker != null) Marker.SetActive(false);
    }
    void OnEnable()
    {
        if (Marker != null) Marker.SetActive(true);
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
