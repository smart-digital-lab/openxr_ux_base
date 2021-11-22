/**********************************************************************************************************************************************************
 * XRUX_Button_Sizer
 * -----------------
 *
 * 2021-08-25
 *
 * A script used primarily by the Editor to resize all the elments in the button from the Inspector UI
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main Class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public class XRUX_Button_Sizer : MonoBehaviour 
{
    public GameObject titleObject;              // The object that contains the textmeshpro object
    public GameObject objectToResize;           // The main visual element to resize


    // ----------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set just the thickness of the button
    // ----------------------------------------------------------------------------------------------------------------------------------------------------------
    public void SetThickness(float thickness)
    {
        if (objectToResize != null)
        {
            SetSize(objectToResize.transform.localScale.x, objectToResize.transform.localScale.y, thickness);
        }
    }
    // ----------------------------------------------------------------------------------------------------------------------------------------------------------



    // ----------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set all the dimensions of the button, and makes sure the text is in the right place
    // ----------------------------------------------------------------------------------------------------------------------------------------------------------
    public void SetSize(float width, float height, float thickness)
    {
        BoxCollider theCollider = transform.GetComponent<BoxCollider>();

        if (objectToResize != null)
        {
            // Main object position and scale
            objectToResize.transform.localPosition = new Vector3(0, 0, -thickness / 2);
            objectToResize.transform.localScale = new Vector3(width, height, thickness);

            // Collider position and scale
            if (theCollider != null)
            {
                theCollider.center = new Vector3(0, 0, -thickness / 2.0f);
                theCollider.size = new Vector3(width, height, thickness);
            }
            objectToResize.transform.localScale = new Vector3(width, height, thickness);
            RectTransform titleRect = (titleObject == null) ? null : titleObject.GetComponent<RectTransform>();

            // Title position, scale
            if (titleObject != null)
            {
                titleObject.transform.localPosition = new Vector3(objectToResize.transform.localPosition.x, objectToResize.transform.localPosition.y, objectToResize.transform.localPosition.z - thickness / 1.96f);

                if (titleRect != null) 
                {
                    titleRect.localScale = new Vector3(height / 10, height / 10, 1);
                }
            }
        }      
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
