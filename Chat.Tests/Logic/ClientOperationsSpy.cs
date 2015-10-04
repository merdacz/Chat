namespace Chat.Tests.Logic
{
    using System.Collections.Generic;

    using Chat.Logic;

    public class ClientOperationsSpy : IClientOperations
    {
        public string LastUserJoined { get; private set; }

        public string LastInvalidMessage { get; private set; }

        public Message LastMessage { get; private set; }

        public string LastUserLeft { get; private set; }

        public bool LeftSuccessfully { get; private set; }

        public bool JoinedSuccessfully { get; private set; }

        public IList<string> LastUsersUponJoin { get; private set; }

        public IList<Message> LastMessagesUponJoin { get; set; }

        public void invalidOperation(string errorMessage)
        {
            this.LastInvalidMessage = errorMessage;
        }

        public void joinedSuccessfully(IEnumerable<string> userList, IEnumerable<Message> recentMessages)
        {
            this.JoinedSuccessfully = true;
            this.LeftSuccessfully = false;
            this.LastUsersUponJoin = new List<string>(userList);
            this.LastMessagesUponJoin = new List<Message>(recentMessages);
        }

        public void leftSuccessfully()
        {
            this.JoinedSuccessfully = false;
            this.LeftSuccessfully = true;
        }

        public void newUserJoined(string username)
        {
            this.LastUserJoined = username;
        }

        public void userLeft(string username)
        {
            this.LastUserLeft = username;
        }

        public void newMessage(string username, string text)
        {
            var message = new Message(username, text);
            this.LastMessage = message;
        }
    }
}