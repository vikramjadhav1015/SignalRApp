using Microsoft.AspNetCore.SignalR;

namespace BlazorServer.Hubs
{
    //Act as server
    public class ChatHub: Hub
    {
        //send message to all clients
        public Task SendMessage(string user, string message)
        { 
            return Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
