/**********************************************************************************************************************************************************
 * XRUX_RadioButton_Sizer
 * ----------------------
 *
 * 2021-08-25
 *
 * A script used primarily by the Editor to resize all the elments in the RadioButton from the Inspector UI
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main Class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public class XRUX_RadioButton_Sizer : MonoBehaviour 
{
    public GameObject titleObject;              // The object that contains the textmeshpro object
    public GameObject objectToResize;           // The main visual element to resize


    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set just the thickness of the knob
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void SetThickness(float thickness)
    {
        if (objectToResize != null)
        {
            SetSize(objectToResize.transform.localScale.x, objectToResize.transform.localScale.y, thickness);
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set all the dimensions of the knob, and makes sure the text is in the right place
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void SetSize(float width, float height, float thickness)
    {
        BoxCollider theCollider = transform.GetComponent<BoxCollider>();
        RectTransform titleRect = (titleObject == null) ? null : titleObject.GetComponent<RectTransform>();

        if (objectToResize != null)
        {
            // Main object position and scale
            objectToResize.transform.localScale = new Vector3(width, height, thickness);
            // Collider position and scale
            if (theCollider != null)
            {
                theCollider.center = new Vector3(0, 0, -thickness / 2.0f);
                theCollider.size = new Vector3(width * 0.75f, height * 0.75f, thickness);
            }
            objectToResize.transform.localPosition = new Vector3(0, 0, 0);

            // Title position, scale
            if (titleObject != null)
            {
                titleObject.transform.localPosition = new Vector3(objectToResize.transform.localPosition.x + width / 1.3f, objectToResize.transform.localPosition.y, -0.002f);

                if (titleRect != null) 
                {
                    titleRect.localScale = new Vector3(height / 10, height / 10, 1);
                }
            }
        }      
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
