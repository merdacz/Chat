namespace Chat.Web.Controllers
{
    using Microsoft.AspNet.SignalR;

    public class ChatHub : Hub
    {
        public void SendMessage(string message)
        {
            this.Clients.All.newMessage(this.Context.ConnectionId, message);
        }
    }
}