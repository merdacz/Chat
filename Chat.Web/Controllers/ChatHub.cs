namespace Chat.Web.Controllers
{
    using Chat.Logic;

    using Microsoft.AspNet.SignalR;

    public class ChatHub : Hub
    {
        private readonly IChat chat;

        private string ConnectionId => this.Context.ConnectionId;

        private static readonly string SystemUserName = "system";


        public ChatHub(IChat chat)
        {
            this.chat = chat;
        }

        public void Join(string username)
        {
            var result = this.chat.Join(username, this.ConnectionId);
            if (result.Success)
            {
                var snapshot = this.chat.TakeSnapshot();
                this.Clients.Caller.joinedSuccessfully(snapshot.Participants, snapshot.RecentMessages);
                this.Clients.Caller.newMessage(SystemUserName, $"Welcome {username} to our SignalR chat. ");
                this.Clients.Others.newMessage(SystemUserName, $"{username} joined the chat. ");
                this.Clients.Others.newUserJoined(username);
                return;
            }

            this.Clients.Caller.invalidOperation(result.ErrorMessage);
        }

        public void Leave()
        {
            var result = this.chat.Leave(this.ConnectionId);
            if (result.Success)
            {
                this.Clients.Caller.leftSuccessfully();
                this.Clients.Others.newMessage(SystemUserName, $"{result.Username} left the chat.");
                this.Clients.All.userLeft(result.Username);
                return;
            }

            this.Clients.Caller.invalidOperation(result.ErrorMessage);
        }

        public void SendMessage(string message)
        {
            var result = this.chat.SendMessage(this.ConnectionId, message);
            if (result.Success)
            {
                this.Clients.All.newMessage(result.Username, message);
                return;
            }

            this.Clients.Caller.invalidOperation(result.ErrorMessage);
        }
    }
}