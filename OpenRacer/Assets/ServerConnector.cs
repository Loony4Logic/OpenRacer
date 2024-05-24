using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System;

public class ServerConnector : MonoBehaviour
{
    ClientWebSocket webSocket = new ClientWebSocket();

    Uri serverUri = new Uri("ws://localhost:8000/ws"); // Replace with your WebSocket server URI
    CancellationTokenSource cts = new CancellationTokenSource();

    // Start is called before the first frame update
    async void Start()
    {
        await webSocket.ConnectAsync(serverUri, cts.Token);
        Debug.Log("Connected to the server");

        // Send a message to the server
        string messageToSend = "track";
        ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(messageToSend));
        await webSocket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, cts.Token);
        Debug.Log($"Sent: {messageToSend}");

        // Receive a message from the server
        ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);
        WebSocketReceiveResult result = await webSocket.ReceiveAsync(bytesReceived, cts.Token);
        string messageReceived = Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count);
        Debug.Log($"Received: {messageReceived}");
    }

    public async void OnDestroy()
    {
        if (webSocket.State == WebSocketState.Open)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cts.Token);
            Debug.Log("Connection closed");
        }
    }

    // Update is called once per frame
    void Update()
    {   
        
    }
}
