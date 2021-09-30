/**********************************************************************************************************************************************************
 * SetScene
 * --------
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


// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Public functions
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public interface _SetScene
{
    void Input(XRData newRotation);
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[AddComponentMenu("OpenXR UX/Tools/Set Scene")]
public class SetScene : MonoBehaviour, _SetScene
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    [Header("____________________________________________________________________________________________________")]
    [Header("Change the Scene and keep the current XRRig.\n____________________________________________________________________________________________________")]
    [Header("INPUTS\n\n - Input() - Scene number to change to.\n - Event 'scene' and Action 'CHANGE' - From the main event queue.")]

    [Header("____________________________________________________________________________________________________")]
    [Header("SETTINGS")]
    [Header("Scene to load on start (-1 for 'stay on this scene')")]
    public int startScene = -1;
    public bool persistAcrossScenes = true; // Whether this gameobject should persist across scenes.

    [Header("____________________________________________________________________________________________________")]
    [Header("OUTPUTS")]
    public UnityXRDataEvent onChange;   // Functions to call when scene is changed.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------


    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private XRDeviceManager watcher;    // The XR Event Manager
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
        // Find the object that has the event manager on it.  It should be the only one with tag XREvents.
        GameObject watcherGameObject = GameObject.FindWithTag("XREvents");
        if (watcherGameObject == null)
            Debug.Log("No XR Device Manager in SceneGraph that is tagged XREvents");
        else
        {
            // Get the script
            watcher = watcherGameObject.GetComponent<XRDeviceManager>();

            // Set up the callback
            watcher.XREventQueue.AddListener(onDeviceEvent);
        }

        // Add the function to call when a scene cahnges
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Load the start scene if required
        if (startScene > 0)
        {
            SceneManager.LoadScene(startScene % SceneManager.sceneCountInBuildSettings);
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set the Scene to the given number
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void Input (XRData sceneNumber)
    {
        Set(sceneNumber.ToInt(), sceneNumber.quietly);
    }
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
    void OnSceneLoaded (Scene scene, LoadSceneMode mode)
    {
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

            // Find the entry point if it exists
            GameObject ENTRY = GameObject.Find("ENTRY");
            if (ENTRY != null)
            {
                this.gameObject.transform.position = ENTRY.transform.position;
            }

            // Slow any movement right down so we don't go zooming into the next scene
            XRCameraMover xrCameraMoverScript = this.gameObject.GetComponent<XRCameraMover>();
            if (xrCameraMoverScript != null)
            {
                xrCameraMoverScript.PutOnBrakes();
                xrCameraMoverScript.StandOnGround();
            }

            // Set the rotation to straight ahead, then back again - this fixes a problem whereby the movement vector became out of sync
            // float yRot = this.gameObject.transform.rotation.eulerAngles.y;
            // this.gameObject.transform.rotation = Quaternion.identity;
            // this.gameObject.transform.rotation = Quaternion.Euler(0, yRot, 0);
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
