namespace Chat.Tests.Logic
{
    using System.Collections.Generic;

    using Chat.Logic;

    public interface IClientOperations
    {
        void invalidOperation(string errorMessage);

        void joinedSuccessfully(IEnumerable<string> userList, IEnumerable<Message> recentMessages);

        void leftSuccessfully();

        void newUserJoined(string username);

        void userLeft(string username);

        void newMessage(string username, string message);
    }
}