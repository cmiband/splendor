using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Text;

namespace SplendorConsole
{
    class WebserviceClient
    {
        private Uri SERVER_ENDPOINT;
        private ClientWebSocket webSocket;

        public WebserviceClient(string endpoint)
        {
            SERVER_ENDPOINT = new Uri(endpoint);
            webSocket = new ClientWebSocket();
        }

        public async Task ConnectToWebsocket()
        {
            await webSocket.ConnectAsync(SERVER_ENDPOINT, CancellationToken.None);
            Console.WriteLine("Connected");
        }

        public async Task SendDataToSocket(string data)
        {
            ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(data));
            await webSocket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task DisconnectFromWebsocket()
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }
    }
}
