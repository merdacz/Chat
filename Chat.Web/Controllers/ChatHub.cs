namespace Chat.Web.Controllers
{
    using System.Collections.Generic;

    using Chat.Logic;

    using Microsoft.AspNet.SignalR;
    using Microsoft.Owin.Security;

 
    public class ChatHub : Hub
    {
        private readonly IMessageLog messageLog;

        public ChatHub(IMessageLog messageLog)
        {
            this.messageLog = messageLog;
        }

        public void SendMessage(string message)
        {
            var username = this.Context.ConnectionId;
            this.messageLog.Save(username, message);
            this.Clients.All.newMessage(username, message);
        }
    }
}