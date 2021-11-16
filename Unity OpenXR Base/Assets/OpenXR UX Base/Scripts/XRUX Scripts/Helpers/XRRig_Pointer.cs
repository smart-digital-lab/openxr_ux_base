/**********************************************************************************************************************************************************
 * XRRig_Pointer
 * -------------
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
public interface IXRRig_Pointer
{
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public class XRRig_Pointer : MonoBehaviour, IXRRig_Pointer
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public GameObject Marker;
    public Material trailMaterial;
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private Vector3 markerOriginalSize;
    private bool isTouching = false;
    public bool IsTouching { get { return isTouching; } }
    private bool isMovingTo = false;
    public bool IsMovingTo { get { return isMovingTo; } }
    private LineRenderer trail;
    private Vector3[] trailPoints;
    private int trailDensity = 20;
// ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set up
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        // Grab the marker object details
        if (Marker != null) 
        {
            markerOriginalSize = Marker.transform.localScale;      
            Marker.SetActive(false);
        }

        // Set up the trail
        trail = gameObject.AddComponent<LineRenderer>();
        trailPoints = new Vector3[trailDensity];
        trail.material = trailMaterial;
        trail.positionCount = trailDensity;
        trail.SetPositions(trailPoints);
        trail.generateLightingData = true;
        trail.enabled = false;
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Raycast from the pointer and move the marker if it hits objects on 'interaction' layers (clickable objects) or 'go' layers (objects to be able to move onto)
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        if (Marker != null)
        {
            Vector3 fwd = transform.TransformDirection(Vector3.forward);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, fwd, out hit, 100, 1<<(int)OpenXR_UX_Layers.OpenXR_UX | 1<<(int)OpenXR_UX_Layers.Go_Areas | 1<<(int)OpenXR_UX_Layers.NoGo_Areas))
            {
                int theLayer = hit.collider.gameObject.layer;
                switch (theLayer)
                {
                    case (int)OpenXR_UX_Layers.OpenXR_UX:
                        Marker.transform.position = hit.point;
                        Marker.transform.localScale = markerOriginalSize;
                        Marker.SetActive(true);
                        isTouching = true;
                        isMovingTo = false;
                        trail.enabled = true;
                        trail.startWidth = 0.005f;
                        trail.endWidth = 0.001f;
                        for (int i = 0; i < trailDensity; i++)
                        {
                            trailPoints[i] = TravelCurve(transform.position, Marker.transform.position, 0.0f, ((i * 1.0f) / (trailDensity  * 1.0f)), Vector3.up);
                        }
                        trail.SetPositions(trailPoints);
                        break;
                    case (int)OpenXR_UX_Layers.Go_Areas:
                        Marker.transform.position = hit.point;
                        Marker.transform.localScale = markerOriginalSize * 30.0f;
                        isTouching = false;
                        isMovingTo = true;
                        Marker.SetActive(true);
                        trail.enabled = true;
                        trail.startWidth = 0.002f;
                        trail.endWidth = 0.1f;
                        for (int i = 0; i < trailDensity; i++)
                        {
                            trailPoints[i] = TravelCurve(transform.position, Marker.transform.position, 0.3f, (((i + 1) * 1.0f) / ((trailDensity + 2) * 1.0f)), Vector3.up);
                        }
                        trail.SetPositions(trailPoints);
                        break;
                    default:
                        Marker.transform.localPosition = Vector3.zero;
                        Marker.transform.localScale = markerOriginalSize;
                        Marker.SetActive(false);
                        isTouching = false;
                        isMovingTo = false;
                        trail.enabled = false;
                        break;
                }
            }
            else
            {
                Marker.transform.localPosition = Vector3.zero;
                Marker.transform.localScale = markerOriginalSize;
                Marker.SetActive(false);                
                trail.enabled = false;
            }
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Work out a curve between start (t=0) and end (t=1)
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    Vector3 TravelCurve(Vector3 start, Vector3 end, float height, float t, Vector3 outDirection)
    {
        float parabolicT = t * 2 - 1;
        //start and end are not level, gets more complicated
        Vector3 travelDirection = end - start;
        Vector3 levelDirection = end - new Vector3(start.x, end.y, start.z);
        Vector3 right = Vector3.Cross(travelDirection, levelDirection);
        Vector3 up = outDirection;
        Vector3 result = start + t * travelDirection;
        result += ((-parabolicT * parabolicT + 1) * height) * up.normalized;
        return result;
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Work out a line between start (t=0) and end (t=1)
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    Vector3 PointerLine(Vector3 start, Vector3 end, float t)
    {
        Vector3 result = Vector3.Lerp(start, end, t);
        return result;
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Make sure the marker is turned on or off when the pointer turns on or off
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void OnDisable()
    {
        if (Marker != null) Marker.SetActive(false);
        if (trail != null) trail.enabled = false;
    }
    void OnEnable()
    {
        if (Marker != null) Marker.SetActive(true);
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
