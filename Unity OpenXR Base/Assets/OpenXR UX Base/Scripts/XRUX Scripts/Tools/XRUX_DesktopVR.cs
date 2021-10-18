using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class XRUX_DesktopVR : MonoBehaviour
{
    public enum XRType { Immersive_XR, Desktop_XR }
    public XRType activateForTypeOfXR = XRType.Immersive_XR;

    // Start is called before the first frame update
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
