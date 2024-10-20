using SplendorConsole;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;

public class Program
{
    public async static Task Main(string[] args)
    {
        WebserviceClient wc = new WebserviceClient("ws://localhost:8080");

        await wc.ConnectToWebsocket();
        await wc.SendDataToSocket("test data");

        /*
        using (ClientWebSocket ws = new ClientWebSocket())
        {
            Uri serverUri = new Uri("ws://localhost:8080");
            await ws.ConnectAsync(serverUri, CancellationToken.None);
            Console.WriteLine("Connected to WebSocket server");

            // Sending a message to the server
            string message = "Hello, server!";
            ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
            await ws.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
            Console.WriteLine($"Sent: {message}");

            // Receiving a message from the server
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
            WebSocketReceiveResult result = await ws.ReceiveAsync(buffer, CancellationToken.None);
            string receivedMessage = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
            Console.WriteLine($"Received: {receivedMessage}");

            // Close the connection
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            Console.WriteLine("Closed WebSocket connection");
        }
        */
    }
}