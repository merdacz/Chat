namespace Chat.Logic
{
    using System.Collections.Generic;

    public class InMemoryMessageLog : IMessageLog
    {
        public static IList<Message> store = new List<Message>();
         
        public void Save(string username, string message)
        {
            var entry = new Message(username, message);
            store.Add(entry);
        }

        public IEnumerable<Message> GetRecentMessages()
        {
            return store;
        }
    }
}