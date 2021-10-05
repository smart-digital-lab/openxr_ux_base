/**********************************************************************************************************************************************************
 * TinyMovements
 * -------------
 *
 * 2021-10-05
 * 
 * Makes tiny rotational and positional movements somewhat randomly whilst moving a texture offset in a particular direction.
 * Quite good to give a large surface representing the sea a sense of movement.
 * 
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main Class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public class TinyMovements : MonoBehaviour
{
    private float startTime;                                        // Start of current time period
    private Quaternion targetRotation = new Quaternion();           // Angle to rotate to in this time period
    private Vector3 targetPosition = new Vector3();                 // Where to move to in this time period
    private Vector3 startPosition;                                  // Start position of the GameObject
    private float timeSpan;                                         // Time until next set of random movements chosen
    private MeshRenderer theRenderer;                               // The object to move
    private Vector2 offset = Vector2.zero;                          // Texture offset



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set things up
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Start()
    {
        startPosition = transform.position;
        theRenderer = this.gameObject.GetComponent<MeshRenderer>();
        ChooseNew();
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Do the slow, tiny movements
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Update()
    {
        // Calculate the physical movement and rotation
        float step = (Time.time - startTime) * 0.01f;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, step);
        transform.position = Vector3.Slerp(transform.position, targetPosition, step);

        // Calculate the texture movement
        float step2 = 0.01f * Time.deltaTime;
        offset = Vector2.MoveTowards(offset, Vector2.one, step2);
        if (offset == Vector2.one) offset = Vector2.zero;

        theRenderer.material.SetTextureOffset("_MainTex", offset);

        // Check if time for a new set of numbers
        if (Time.time-startTime > timeSpan)
        {
            ChooseNew();
        }  
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------


    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Choose some new random numbers
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void ChooseNew()
    {
        Quaternion newQuat = new Quaternion();
        newQuat.eulerAngles = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
        targetRotation = newQuat;

        targetPosition = startPosition + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));

        timeSpan = Random.Range(1.0f, 4.0f);
        startTime = Time.time;
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------