﻿namespace Chat.Web.Controllers
{
    using System.Collections.Generic;
    using System.Web;

    using Chat.Logic;

    using Microsoft.AspNet.SignalR;
 
    public class ChatHub : Hub
    {
        private readonly IMessageLog messageLog;

        private readonly IChatConfiguration configuration;

        private string ConnectionId => this.Context.ConnectionId;

        private static readonly string SystemUserName = "system";

        private static readonly IDictionary<string, string> Participants = new Dictionary<string, string>();
         
        public ChatHub(IMessageLog messageLog, IChatConfiguration configuration)
        {
            this.messageLog = messageLog;
            this.configuration = configuration;
        }

        public void Join(string username)
        {
            var usernameEncoded = HttpUtility.HtmlEncode(username);
            if (Participants.ContainsKey(this.ConnectionId))
            {
                this.Clients.Caller.invalidOperation($"Already logged in as {Participants[this.ConnectionId]}, please leave chat first. ");
                return;
            }

            if (Participants.Values.Contains(usernameEncoded))
            {
                this.Clients.Caller.invalidOperation("Name already taken, please choose another one. ");
                return;
            }

            if (Participants.Count >= this.configuration.GetMaxCapacity())
            {
                this.Clients.Caller.invalidOperation("Chat is full, please retry. ");
                return;
            }

            Participants.Add(this.ConnectionId, usernameEncoded);

            this.Clients.Caller.joinedSuccessfully(Participants.Values, this.messageLog.GetRecentMessages()); 
            this.Clients.Caller.newMessage(SystemUserName, $"Welcome {usernameEncoded} to our SignalR chat. ");
            this.Clients.Others.newMessage(SystemUserName, $"{usernameEncoded} joined the chat. ");
            this.Clients.Others.newUserJoined(usernameEncoded);
        }

        public void Leave()
        {
            if (!Participants.ContainsKey(this.ConnectionId))
            {
                this.Clients.Caller.ivalidOperation("Already left the chat. ");
                return;
            }

            string username = Participants[this.ConnectionId];
            Participants.Remove(this.ConnectionId);

            this.Clients.Caller.leftSuccessfully();
            this.Clients.Caller.newMessage(SystemUserName, $"Bye bye, {username}. We hope you will re-join the chat soon. ");
            this.Clients.Others.newMessage(SystemUserName, $"{username} left the chat." );
            this.Clients.All.userLeft(username);
        }

        public void SendMessage(string message)
        {
            var messageEncoded = HttpUtility.HtmlEncode(message);
            if (!Participants.ContainsKey(this.ConnectionId))
            {
                this.Clients.Caller.invalidOperation("Cannot send messages until joined. ");
                return;
            }

            string username = Participants[this.ConnectionId];
            this.messageLog.Save(username, messageEncoded);
            this.Clients.All.newMessage(username, messageEncoded);
        }
    }
}