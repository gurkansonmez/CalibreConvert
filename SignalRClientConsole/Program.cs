using Microsoft.AspNetCore.SignalR.Client;
using System;
namespace SignalRClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5001/chatHub")
                .Build();

            connection.StartAsync().Wait();
            connection.InvokeAsync("SendMessage", arg1: new[] { "Scribo", "Hello" });
            connection.On("ReceiveMessage", (string userName, string message) =>
            {
                Console.WriteLine(userName + ':' + message);
            });
        }
    }
}