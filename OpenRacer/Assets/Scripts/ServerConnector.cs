using UnityEngine;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ServerConnector 
{
    Queue<string> messages = new Queue<string>();
    ClientWebSocket webSocket = new ClientWebSocket();
    Uri serverUri = new Uri("ws://localhost:8000/ws"); 
    // TODO: Create settings to change with custom WebSocket server URI
    CancellationTokenSource cts = new CancellationTokenSource();

    async public Task<bool> Start()
    {
        await webSocket.ConnectAsync(serverUri, cts.Token);
        Debug.Log("Connected to the server");

        return true;
    }

    public async void OnDestroy()
    {
        if (webSocket.State == WebSocketState.Open)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cts.Token);
            Debug.Log("Connection closed");
        }
    }

    public async Task<String> sendToWebsocket(string messageToSend)
    {
        if (string.IsNullOrEmpty(messageToSend)) return null;
        //TODO: add a check to see if there is any waiting in msg
        // Send a message to the server
        ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(messageToSend));
        await webSocket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, cts.Token);
        Debug.Log($"Sent: {messageToSend}");

        // Receive a message from the server
        ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024 * 50]);
        WebSocketReceiveResult result = await webSocket.ReceiveAsync(bytesReceived, cts.Token);
        string messageReceived = Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count);
        Debug.Log($"Received: {messageReceived}");
        return messageReceived;
    }
}
