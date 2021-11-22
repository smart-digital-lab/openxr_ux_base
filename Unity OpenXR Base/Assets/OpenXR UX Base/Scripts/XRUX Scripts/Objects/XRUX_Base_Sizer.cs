/**********************************************************************************************************************************************************
 * XRUX_Button_Sizer
 * -----------------
 *
 * 2021-08-25
 *
 * A script used primarily by the Editor to resize all the elments in the base object from the Inspector UI
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main Class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public class XRUX_Base_Sizer : MonoBehaviour 
{
    public GameObject theBase;              // The main base object
    public GameObject minimiseButton;       // The top of the minimise button heirarchy
    public GameObject maximiseButton;       // The top of the maximise button heirarchy
    public GameObject theTitlebar;          // The titlebar object
    public GameObject theTitle;             // The title that has the textmeshpro object on it


    // ----------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set all the dimensions of the base object, and makes sure the text is in the right place
    // ----------------------------------------------------------------------------------------------------------------------------------------------------------
    public void SetSize(float width, float height, float thickness)
    {
        RectTransform titleRect = (theTitle == null) ? null : theTitle.GetComponent<RectTransform>();
        XRUX_Button_Sizer minButtonScriptSizer = (minimiseButton == null) ? null : minimiseButton.GetComponent<XRUX_Button_Sizer>();
        XRUX_Button_Sizer maxButtonScriptSizer = (maximiseButton == null) ? null : maximiseButton.GetComponent<XRUX_Button_Sizer>();

        if (theBase != null)
        {
            if (theTitlebar != null)
            {
                // Set the base position and size
                theBase.transform.localScale = new Vector3(width, height - theTitlebar.transform.localScale.y, thickness);
                theBase.transform.localPosition = new Vector3(0, 0, -thickness / 2);

                // Set the titlebar and minimise button position and size
                if (minimiseButton != null)
                {
                    float buttonWidth = minButtonScriptSizer.objectToResize.transform.localScale.x;
                    theTitlebar.transform.localScale = new Vector3(width - buttonWidth, theTitlebar.transform.localScale.y, thickness);
                    theTitlebar.transform.localPosition = new Vector3(theBase.transform.localPosition.x - buttonWidth / 2, height / 2, -thickness / 2);
                    if (minButtonScriptSizer != null) minButtonScriptSizer.SetThickness(thickness);

                    if (theTitle != null)
                    {
                        theTitle.transform.localPosition = new Vector3(theBase.transform.localPosition.x - buttonWidth / 2, height / 2 - theTitlebar.transform.localScale.y / 2 + 0.001f, theBase.transform.localPosition.z - theTitlebar.transform.localScale.z / 2.0f - 0.0002f);
                    }
                    if (titleRect != null) 
                    {
                        titleRect.sizeDelta = new Vector2(theTitlebar.transform.localScale.x / titleRect.localScale.x, 1);
                    }
                    minimiseButton.transform.localPosition = new Vector3(width / 2 - buttonWidth / 2, height / 2, 0);
                }

                // Set the maximise button position and size, allowing the x and y values to stay as the user has set them
                if (maximiseButton != null)
                {
                    maximiseButton.transform.localPosition = new Vector3(maximiseButton.transform.localPosition.x, maximiseButton.transform.localPosition.y, 0);
                    if (maxButtonScriptSizer != null) maxButtonScriptSizer.SetThickness(thickness);
                }
            }
        }        
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
