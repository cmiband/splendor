using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;

public delegate void FetchDataCallbackHandler(string data);

public class WebServiceClient : MonoBehaviour
{
    private const string CONNECTION_ERROR_MESSAGE = "Error occured while trying to connect to web socket";
    private const string SEND_DATA_EXCEPTION_MESSAGE = "Error occured while trying to send data to web socket";
    private const string SEND_AND_FETCH_EXCEPTION_MESSAGE = "Error occured while trying to send and fetch data from web socket";
    private const string FETCH_DATA_EXCEPTION_MESSAGE = "Error occured while trying to fetch data from web socket";
    private const string FETCH_DATA_WITH_CALLBACK_EXCEPTION_MESSAGE = "Error occured while trying to fetch data and perform callback";
    private const string DISCONNECT_ERROR_MESSAGE = "Error occured while trying to disconnect from web socket";
    private const string CLOSING_STATUS = "Closing";
    public static Uri serverEndpoint = new Uri("ws://localhost:8765");
    public static ClientWebSocket? webSocket;

    public static void InitSocket()
    {
        webSocket = new ClientWebSocket();
    }

    public static void ClearSocket()
    {
        webSocket = null;
    }

    public static async Task ConnectToWebsocket()
    {
        try
        {
            await webSocket.ConnectAsync(serverEndpoint, CancellationToken.None);
        }
        catch (Exception e)
        {
            Debug.LogError(System.DateTime.Now+"  "+e.Message);
            throw new WebserviceClientException(CONNECTION_ERROR_MESSAGE, e);
        }
    }

    public static bool CheckIfSocketIsConnected()
    {
        return webSocket.State == WebSocketState.Open;
    }

    public static async Task SendDataToSocket(string data)
    {
        try
        {
            ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(data));
            await webSocket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
        }
        catch (Exception e)
        {
            Console.WriteLine(SEND_DATA_EXCEPTION_MESSAGE + " " + e.Message + " " + e.StackTrace);
        }
    }

    public static async Task<string> SendAndFetchDataFromSocket(string data)
    {
        try
        {
            await SendDataToSocket(data);

            return await FetchDataFromSocket();
        }
        catch (Exception e)
        {
            Console.WriteLine(SEND_AND_FETCH_EXCEPTION_MESSAGE + " " + e.Message + " " + e.StackTrace);
        }

        return "";
    }

    public static async Task<string> FetchDataFromSocket()
    {
        try
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
            string receivedMessage = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);

            return receivedMessage;
        }
        catch (Exception e)
        {
            Console.WriteLine(FETCH_DATA_EXCEPTION_MESSAGE + " " + e.Message + " " + e.StackTrace);
        }
        return "";
    }

    public static async Task FetchDataFromSocketAndPerformCallback(FetchDataCallbackHandler callback)
    {
        try
        {
            string data = await FetchDataFromSocket();
            if (callback != null)
            {
                callback(data);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(FETCH_DATA_WITH_CALLBACK_EXCEPTION_MESSAGE + " " + e.StackTrace);
        }
    }

    public static async Task DisconnectFromWebsocket()
    {
        try
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, CLOSING_STATUS, CancellationToken.None);
        }
        catch (Exception e)
        {
            throw new WebserviceClientException(DISCONNECT_ERROR_MESSAGE, e);
        }
    }

    public class WebserviceClientException : Exception
    {
        public WebserviceClientException(string message, Exception baseException) : base(message, baseException) { }
    }
}
