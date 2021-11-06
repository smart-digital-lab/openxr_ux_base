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
    private string endpoint = "ws://" + serverAddress + ":8080/comms";
    private Task receiveTask;

    private bool commandReady = false;
    private Vector3 newRotation;


    // Start is called before the first frame update
    async void Start()
    {
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
            String connection_message = "{\"connect\":[\"Fred\"]}";
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
                        string message = reader.ReadToEnd();
                        // Debug.Log (message);

                        Hashtable messageHash = (Hashtable) JSON.JsonDecode(message);
                        if (messageHash == null) break;
                        ArrayList messageData = (ArrayList) messageHash["data"];
                        if (messageData == null) break;
                        switch ((string) messageData[2])
                        {
                            case "rotation": 
                                // Debug.Log("rotating");
                                ArrayList messageRotation = (ArrayList) messageData[4];
                                if (messageRotation == null) break;
                                
                                newRotation = Convert2Vector3(messageRotation);
                                // Debug.Log(newRotation);
                                commandReady = true;
                                break;
                            default:
                                break;
                        }                       
                    }
                }
            }
        }
        Debug.Log("Websocket Closed");
    }



    Vector3 Convert2Vector3(ArrayList data)
    {
        float x, y, z = 0.0f;
        float.TryParse(data[0].ToString(), out x);
        float.TryParse(data[1].ToString(), out y);
        float.TryParse(data[2].ToString(), out z);
        return new Vector3(x,y,z);
    }



    // Update is called once per frame
    void Update()
    {
        if (commandReady)
        {
            transform.rotation = Quaternion.Euler(newRotation);
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
