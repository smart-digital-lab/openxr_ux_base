/**********************************************************************************************************************************************************
 * XRUX_DesktopXR
 * --------------
 *
 * 2021-11-15
 *
 * Activate or inactivate an object depending on whether running in Desktop or Immersive VR.
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[AddComponentMenu("OpenXR UX/Tools/XRUX Desktop XR")]
public class XRUX_DesktopXR : MonoBehaviour
{
    public enum XRType { Immersive_XR, Desktop_XR }
    public XRType activateForTypeOfXR = XRType.Immersive_XR;

    void Start()
    {
        if (activateForTypeOfXR == XRType.Immersive_XR)
        {
            if (XRSettings.isDeviceActive) 
                this.gameObject.SetActive(true);
            else
                this.gameObject.SetActive(false);
        }
        else
        {
            if (!XRSettings.isDeviceActive)
                this.gameObject.SetActive(true);
            else
                this.gameObject.SetActive(false);
        }
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
