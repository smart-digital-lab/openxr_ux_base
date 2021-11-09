using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRMuxClient : MonoBehaviour
{
    public bool receive = true;
    public bool send = true;

    private float prevTime;

    private Vector3 newLocalEulerAngles;

    private Vector3 prevLocalRotationEuler = new Vector3();
    // Start is called before the first frame update
    void Start()
    {
        // Listen for events coming from the WebSocket
        if (XRMux.EventQueue != null) XRMux.EventQueue.AddListener(onDeviceEvent);
        prevTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float step =  1.0f * Time.deltaTime;
        transform.localEulerAngles = Vector3.MoveTowards(transform.localEulerAngles, newLocalEulerAngles, step);
        
        if (send && ((Time.time - prevTime) > 0.1f))
        {
            if (transform.localEulerAngles != prevLocalRotationEuler)
            {
                XRMuxData newData = new XRMuxData(name, "localrotationeuler", transform.localEulerAngles, XRMuxData.XRMuxDataDirection.OUT);
                XRMuxEvent eventToSend = new XRMuxEvent();
                eventToSend.data = newData;

                XRMux.EventQueue.Invoke(eventToSend);
                prevLocalRotationEuler = transform.localEulerAngles;
            }
            prevTime = Time.time;
        }
    }


    
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // What to do with the incoming data
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    private void onDeviceEvent(XRMuxEvent theEvent)
    {
        if (receive) Debug.Log("received " + theEvent.data.direction);
        if (receive && theEvent.data.direction == XRMuxData.XRMuxDataDirection.IN)
        {
            if (theEvent.data.objectName == name)
            {
                switch (theEvent.data.objectParameter)
                {
                    case "position":
                        transform.position = theEvent.data.ToVector3();
                        break;
                    case "rotation":
                        break;
                    case "rotationeuler":
                        transform.eulerAngles = theEvent.data.ToVector3();
                        break;
                    case "localposition":
                        transform.localPosition = theEvent.data.ToVector3();
                        break;
                    case "localrotation":
                        break;
                    case "localrotationeuler":
                        newLocalEulerAngles = theEvent.data.ToVector3();
                        break;
                    default:
                        Debug.Log ("Received uncaught event for " + theEvent.data.objectName);
                        Debug.Log ("    data for " + theEvent.data.objectParameter);
                        break;
                }
            }
        }
    }
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
}
