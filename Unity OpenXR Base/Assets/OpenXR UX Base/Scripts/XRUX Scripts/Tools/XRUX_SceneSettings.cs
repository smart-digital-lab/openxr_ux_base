/**********************************************************************************************************************************************************
 * XRUX_SceneSettings
 * ------------------
 *
 * 2021-10-05
 *
 * Contains data to be used by CameraMover on SceneLoad about the visual quality settings.
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


public enum SceneSettingsAntiAliasing    { None, TwoTimes, FourTimes, EightTimes}
public enum SceneSettingsTextureQuality  { Eighth, Quarter, Half, Full }
public enum SceneSettingsVisualQuality   { Low, Medium, Normal, High, Extra }


// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Public functions
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public interface _XRUX_SceneSettings
{
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[AddComponentMenu("OpenXR UX/Tools/XRUX Scene Settings")]
public class XRUX_SceneSettings : MonoBehaviour, _XRUX_SceneSettings
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public bool dynamicQuality = true;
    public SceneSettingsAntiAliasing movingAntiAliasingLevel = SceneSettingsAntiAliasing.None;
    public SceneSettingsTextureQuality movingTextureQuality = SceneSettingsTextureQuality.Eighth;
    public SceneSettingsVisualQuality movingVisualQuality = SceneSettingsVisualQuality.Medium;
    public ShadowQuality movingShadowQuality = ShadowQuality.Disable;
    public ShadowResolution movingShadowResolution = ShadowResolution.Low;
    public SceneSettingsAntiAliasing standingAntiAliasingLevel = SceneSettingsAntiAliasing.EightTimes;
    public SceneSettingsTextureQuality standingTextureQuality = SceneSettingsTextureQuality.Full;
    public SceneSettingsVisualQuality standingVisualQuality = SceneSettingsVisualQuality.High;
    public ShadowQuality standingShadowQuality = ShadowQuality.All;
    public ShadowResolution standingShadowResolution = ShadowResolution.VeryHigh;
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
