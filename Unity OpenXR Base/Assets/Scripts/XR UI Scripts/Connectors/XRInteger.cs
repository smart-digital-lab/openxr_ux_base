/**********************************************************************************************************************************************************
 * XRInteger
 * ---------
 *
 * 2021-09-05
 *
 * Sends an Integer when told to.
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Public functions
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public interface _XRInteger
{
    void Input();
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[AddComponentMenu("XR/UX/Connectors/XRInteger")]
public class XRInteger : MonoBehaviour, _XRInteger
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    [Header("____________________________________________________________________________________________________")]
    [Header("Generate a Integer variable for XRData.\n____________________________________________________________________________________________________")]
    [Header("INPUTS\n\n - Input() - Trigger to activate function.")]

    [Header("____________________________________________________________________________________________________")]
    [Header("SETTINGS")]
    [Header("The Integer variable.")]
    public int value;

    [Header("____________________________________________________________________________________________________")]
    [Header("OUTPUTS")]
    public UnityXRDataEvent onChange;
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Input values of on or off
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void Input ()
    {
        if (onChange != null) onChange.Invoke(new XRData(value));
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
