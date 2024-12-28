using System;
using System.Threading.Tasks;
using Quobject.SocketIoClientDotNet.Client;

internal class SocketIOOptions : IO.Options
{
    public bool Reconnection { get; set; }
    public int ReconnectionDelay { get; set; }
    public int EIO { get; set; }
}
class Program
{
    static void Main(string[] args)
    {
        ConnectSocketIO();
        Console.ReadLine(); // Keep the application running
    }

    private static void ConnectSocketIO()
    
    {
        var options = new SocketIOOptions() { Reconnection = true, ReconnectionDelay = 250, EIO = 4};

        var socket = IO.Socket("http://127.0.0.1:8765", options);

        //socket.JsonSerializer = new NewtonsoftJsonSerializer(socket.Options.EIO);

        socket.On(Socket.EVENT_CONNECT, () =>
        {
            Console.WriteLine("Connected to the Socket.IO server.");
            socket.Emit("message", "Hello from C# client!");
        });

        socket.On("message", (data) =>
        {
            Console.WriteLine("Received: " + data);
        });

        socket.On(Socket.EVENT_DISCONNECT, () =>
        {
            Console.WriteLine("Disconnected from the server.");
        });

        socket.On(Socket.EVENT_ERROR, (error) =>
        {
            Console.WriteLine("Error: " + error);
        });
    }
}

