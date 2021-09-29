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
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[AddComponentMenu("OpenXR UX/Tools/Set Scene")]
public class SetScene : MonoBehaviour, _SetScene
{
    [Header("____________________________________________________________________________________________________")]
    [Header("SETTINGS")]
    public GameObject teleportFader;

    [Header("____________________________________________________________________________________________________")]
    [Header("OUTPUTS")]
    public UnityXRDataEvent onChange; // Functions to call when scene is changed.
    public UnityXRDataEvent onLoad; // Functions to call when scene is loaded.

    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Public variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Private variables
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private XRDeviceManager watcher; // The XR Event Manager
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Make sure this gameobject is not removed - it goes on the main XRRig
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
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

        // if (SceneManager.sceneCount > 1)
        // {
            SceneManager.sceneLoaded += OnSceneLoaded;
            //StartCoroutine(LoadAsyncScene(1));
            SceneManager.LoadScene(1);
        // }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    IEnumerator LoadAsyncScene(int sceneNumber)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNumber);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set the scene to the given sceneNumber 
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public void Set(XRData sceneNumber)
    {
        Set(sceneNumber.ToInt());
    }
    public void Set(int sceneNumber)
    {
        // if (SceneManager.sceneCount > sceneNumber)
        // // {
        //     if (teleportFader != null)
        //     {
        //         Renderer teleportFaderRenderer = teleportFader.GetComponent<Renderer>();
        //         if (teleportFaderRenderer != null)
        //         {
        //             Material theMaterial = teleportFaderRenderer.material;
        //             theMaterial.SetColor("_Color", new Color(theMaterial.color.r, theMaterial.color.r, theMaterial.color.r, 1.0f));
        //             teleportFader.SetActive(true);
        //         }
        //     }

            if (onChange != null) onChange.Invoke(new XRData(sceneNumber));
            SceneManager.LoadScene(sceneNumber);
            // StartCoroutine(LoadAsyncScene(sceneNumber));
        // }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------



    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // When the Scene is loaded...
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    void OnSceneLoaded (Scene scene, LoadSceneMode mode)
    {
        GameObject ENTRY = GameObject.Find("ENTRY");
        if (ENTRY != null)
        {
            this.gameObject.transform.position = ENTRY.transform.position;
        }

        // if (teleportFader != null)
        // {
        //     Renderer teleportFaderRenderer = teleportFader.GetComponent<Renderer>();
        //     if (teleportFaderRenderer != null)
        //     {
        //         Material theMaterial = teleportFaderRenderer.material;
        //         theMaterial.SetColor("_Color", new Color(theMaterial.color.r, theMaterial.color.r, theMaterial.color.r, 0.0f));
        //         teleportFader.SetActive(false);
        //     }
        // }

        this.gameObject.transform.rotation = Quaternion.identity;
        if (onLoad != null) onLoad.Invoke(new XRData(scene.buildIndex));
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
