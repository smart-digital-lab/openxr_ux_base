/**********************************************************************************************************************************************************
 * Websockets
 * ----------
 *
 * 2021-11-09
 *
 * The main class for dealing with WebSockets and Unity Events to send and receive data to and from the websocket to the MRMux Server.
 *
 * Roy Davies, Smart Digital Lab, University of Auckland.
 **********************************************************************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Net.WebSockets;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using UnityEngine.Events;
using JSONEncoderDecoder;



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// The type of data that can be sent via the XRMux Event
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[Serializable]
public class XRMuxEvent
{
    public XRMuxData data;
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// The unity event queue for XRMux events
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
[Serializable]
public class XRMuxEvents : UnityEvent<XRMuxEvent> { };
// ----------------------------------------------------------------------------------------------------------------------------------------------------------



// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Main Class
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public class Websockets : MonoBehaviour
{

    public XRMuxEvents XRMuxEventQueue; // The event queue that all the XRMux elements will need to look at to get events.

    public string serverAddress = "localhost";
    public string serverPort = "8810";
    private ClientWebSocket socket = new ClientWebSocket();
    private string endpoint;
    private string productName;
    private Task receiveTask;

    private bool eventReady = false;
    private XRMuxData newData;


    // Start is called before the first frame update
    async void Start()
    {
        endpoint = "ws://" + serverAddress + ":" + serverPort + "/comms";
        productName = Application.companyName + "." + Application.productName + "." + Application.version;
        if (XRMux.EventQueue != null) XRMux.EventQueue.AddListener(onDeviceEvent);

        await Initialize();
    }


    public async Task Initialize()
    {
        await OpenConnection();
    }

    public async Task OpenConnection()
    {
        if (socket.State != WebSocketState.Open)
        {
            await socket.ConnectAsync(new Uri(endpoint), CancellationToken.None);
            Debug.Log("Websocket Opened");
            Task connectTask = Task.Run(async () => await Connect());
            receiveTask = Task.Run(async () => await Receive());
        }
    }

    private async Task Connect()
    {
        if (socket.State == WebSocketState.Open)
        {
            String connection_message = "{\"connect\":[\"" + productName + "\"]}";
            await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(connection_message)), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

    private async Task Receive()
    {
        while (socket.State == WebSocketState.Open)
        {
            byte[] buffer = new byte[1024];
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                break;
            }
            else
            {
                using (var stream = new MemoryStream())
                {
                    stream.Write(buffer, 0, result.Count);
                    while (!result.EndOfMessage)
                    {
                        result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        stream.Write(buffer, 0, result.Count);
                    }

                    stream.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        // Raw Message
                        string message = reader.ReadToEnd();

                        Debug.Log(message);

                        // Message to JSON format - should be a JSON object { "command": data }
                        Hashtable messageHash = (Hashtable) JSON.JsonDecode(message);
                        if (messageHash == null) break;

                        if (messageHash["data"] != null)
                        {
                            // Data should be a JSON array with 5 items [appname, objectname, parameter, type, value]
                            ArrayList messageData = (ArrayList) messageHash["data"];
                            if (messageData == null) break;
                            if (messageData.Count < 5) break;

                            newData = new XRMuxData(messageData, XRMuxData.XRMuxDataDirection.IN);
                            eventReady = true;                            
                        }            
                    }
                }
            }
        }
        Debug.Log("Websocket Closed");
    }



    private void onDeviceEvent(XRMuxEvent theEvent)
    {
        if (theEvent.data.direction == XRMuxData.XRMuxDataDirection.OUT)
        {
            if (socket.State == WebSocketState.Open)
            {
                string message = "{\"data\":[\"" + productName + "\",\"" + theEvent.data.objectName + "\",\"" + theEvent.data.objectParameter + "\",\"" + theEvent.data.GetTypeString() + "\"," + theEvent.data.ToString() + "]}";
                Task sendTask = Task.Run(async () => await Send(message));
            }
        }
    }

    private async Task Send(string message)
    {
        // Debug.Log(message);
        await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text, true, CancellationToken.None);        
    }



    void Update()
    {
        if (eventReady)
        {
            Debug.Log("Data being sent to object");
            XRMuxEvent eventToSend = new XRMuxEvent();
            eventToSend.data = newData;

            XRMuxEventQueue.Invoke(eventToSend);
            eventReady = false;
        }
    }
    

    async void OnDestroy()
    {
        if (socket.State == WebSocketState.Open)
        {
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            receiveTask.Wait();
        }
    }
}


// ----------------------------------------------------------------------------------------------------------------------------------------------------------
// Some useful static helper functions
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
public static class XRMux
{
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    // Find the current Event Queue, if there is one.  Returns null if there isn't one, or the queue if it finds it.
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    public static XRMuxEvents EventQueue
    {
        get
        {
            // Find the object that has the event manager on it.  It should be the only one with tag XREvents.
            GameObject WebSocketGameObject = GameObject.FindWithTag(Enum.GetName(typeof(OpenXR_UX_Tags), OpenXR_UX_Tags.XRMuxEvents));
            if (WebSocketGameObject == null)
            {
                Debug.Log("There is no XRMux Websockets Manager in the SceneGraph that is tagged XRMuxEvents.");
                return null;
            }
            else
            {
                Websockets websockets = WebSocketGameObject.GetComponent<Websockets>();
                if (websockets == null)
                {
                    return null;
                }
                else
                {
                    return websockets.XRMuxEventQueue;
                }
            }
        }
    }
}
// ----------------------------------------------------------------------------------------------------------------------------------------------------------
