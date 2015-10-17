namespace Chat.Logic.Log
{
    using System.Collections.Generic;

    public interface IMessageLog
    {
        void Save(string username, string message);

        IEnumerable<Message> GetRecentMessages();
    }
}