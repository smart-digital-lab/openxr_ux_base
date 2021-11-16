/**********************************************************************************************************************************************************
 * XRUX_SetScene
 * -------------
 *
 * 2021-09-26
 *
 * A tool to change the scene either on command or from an XREvent.
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;


// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Public functions
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public interface IXRUX_SetScene
{
    void Input(XRData sceneNumber);
    // void Quality (XRData visualQuality);
    // void Antialias (XRData antiAlias);
    // void TextureLevel (XRData textureLevel);
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[AddComponentMenu("OpenXR UX/Tools/XRUX Set Scene")]
public class XRUX_SetScene : MonoBehaviour, IXRUX_SetScene
{
    public enum VisualQuality {low, medium, normal, high, extra}

    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public int startScene = -1;
    public bool persistAcrossScenes = true; // Whether this gameobject should persist across scenes.
    public UnityXRDataEvent onChange;   // Functions to call when scene is changed.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------


    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    VisualQuality visualQuality = VisualQuality.normal;
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Make sure this gameobject is not removed if on the XRRig - it goes on the main XRRig by default, but can also be used elsewhere.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Awake()
    {
        if (persistAcrossScenes)
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // On the start
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Start()
    {               
        // Listen for events coming from the XR Controllers and other devices
        if (XRRig.EventQueue != null) XRRig.EventQueue.AddListener(onDeviceEvent);

        // Add the function to call when a scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Load the start scene if required
        if (startScene >= 0)
        {
            SceneManager.LoadScene(startScene % SceneManager.sceneCountInBuildSettings);
        }
            
        // Set the starting position
        SetStartPosition();
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set the Scene to the given number or the quality to the given level
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void Input (XRData sceneNumber)
    {
        Set(sceneNumber.ToInt(), sceneNumber.quietly);
    }
    // public void Quality (XRData quality)
    // {
    //     visualQuality = (VisualQuality) (quality.ToInt() % 5);
    // }
    // public void Antialias (XRData antiAlias)
    // {
    //     switch (antiAlias.ToInt())
    //     {
    //         case 0: QualitySettings.antiAliasing = 0; break;
    //         case 1: QualitySettings.antiAliasing = 2; break;
    //         case 3: QualitySettings.antiAliasing = 8; break;
    //         default: QualitySettings.antiAliasing = 4; break;
    //     }
    // }
    // public void TextureLevel (XRData textureLevel)
    // {
    //     QualitySettings.masterTextureLimit = textureLevel.ToInt() % 4;
    // }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set the scene to the given sceneNumber 
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void Set(int sceneNumber, bool quietly = false)
    {
        if ((onChange != null) && !quietly) onChange.Invoke(new XRData(sceneNumber % SceneManager.sceneCountInBuildSettings));
        SceneManager.LoadScene(sceneNumber % SceneManager.sceneCountInBuildSettings);
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // When the Scene is loaded, remove any other camera, set up the view and call other onLoad functions
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void OnSceneLoaded (Scene scene, LoadSceneMode mode)
    {
        XRSettings.eyeTextureResolutionScale = (int)visualQuality / 4.0f + 0.5f;

        // Remove any other camera objects (eg an existing XRRig or some test camera), though not the one this script is on if it is the only one.
        if (persistAcrossScenes)
        {
            Camera[] cameras = Camera.allCameras;
            foreach (Camera camera in cameras)
            {
                if (camera.gameObject.transform.root.gameObject != this.gameObject)
                {
                    DestroyImmediate(camera.gameObject.transform.root.gameObject);
                }               
            }

            // Set the starting position
            SetStartPosition();
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set the ENTRY point
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void SetStartPosition()
    {
        XRRig_CameraMover xrCameraMoverScript = this.gameObject.GetComponent<XRRig_CameraMover>();

        if (xrCameraMoverScript != null)
        {
            // Find the entry point if it exists
            GameObject ENTRY = GameObject.Find(xrCameraMoverScript.sceneSettingsObjectName);
            if (ENTRY != null)
            {
                this.gameObject.transform.position = ENTRY.transform.position;
            }

            // Slow any movement right down so we don't go zooming into the next scene
            xrCameraMoverScript.PutOnBrakes();
            xrCameraMoverScript.StandOnGround();
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Change the Scene from an XREvent
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void onDeviceEvent(XREvent theEvent)
    {
        if ((theEvent.eventType == XRDeviceEventTypes.scene) && (theEvent.eventAction == XRDeviceActions.CHANGE))
        {
            Set (theEvent.data.ToInt());
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
