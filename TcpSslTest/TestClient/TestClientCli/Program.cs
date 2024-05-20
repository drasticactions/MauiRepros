// See https://aka.ms/new-console-template for more information

using TestClientCore;

Console.WriteLine("Enter port");

var port = Console.ReadLine();

if (string.IsNullOrEmpty(port))
{
    Console.WriteLine("Port is required");
    return;
}

Console.WriteLine("Connecting to localhost");
var client = new TcpTestClient();
await client.ConnectAsync("localhost", int.Parse(port));

while (client.Client.Connected)
{
    Console.WriteLine("Write Message...");
    var message = Console.ReadLine();
    if (!string.IsNullOrEmpty(message))
    {
        await client.Send(message);
    }
}