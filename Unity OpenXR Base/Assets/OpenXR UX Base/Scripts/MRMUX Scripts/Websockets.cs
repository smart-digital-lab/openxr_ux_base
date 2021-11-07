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
using JSONEncoderDecoder;

public class Websockets : MonoBehaviour
{
    public string serverAddress = "localhost";
    private ClientWebSocket socket = new ClientWebSocket();
    private string endpoint;
    private string productName;
    private Task receiveTask;

    private bool commandReady = false;
    private XRMXData newData;


    // Start is called before the first frame update
    async void Start()
    {
        endpoint = "ws://" + serverAddress + ":8080/comms";
        productName = Application.companyName + "." + Application.productName + "." + Application.version;
        Debug.Log(productName);

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

                            newData = new XRMXData(messageData, XRMXData.XRMXDataDirection.IN);
                            commandReady = true;                            
                        }

                        // Get the first key
                        // string command = "";
                        // IList iList = messageHash.Keys as IList;
                        // if (iList != null)
                        // {
                        //     command = (string) iList[0];
                        // }

                        // Debug.Log(command);

                        // switch (command)
                        // {
                        //     case "data":
                        //         // Data should be a JSON array with 5 items [appname, objectname, parameter, type, value]
                        //         ArrayList messageData = (ArrayList) messageHash["data"];
                        //         if (messageData == null) break;

                        //         newData = new XRMXData(messageData, XRMXData.XRMXDataDirection.IN);
                        //         commandReady = true;

                        //         break;
                        //     default:
                        //         break;
                        // }                 
                    }
                }
            }
        }
        Debug.Log("Websocket Closed");
    }



    // Update is called once per frame
    void Update()
    {
        if (commandReady)
        {
            Debug.Log("Data being sent to object");
            //XRMXEventQueue.Invoke(newData);
            commandReady = false;
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
