namespace Chat.Logic
{
    using System.Collections.Generic;

    public class InMemoryMessageLog : IMessageLog
    {
        public static IList<Message> store = new List<Message>();

        public static object syncLock = new object();
         
        public void Save(string username, string message)
        {
            var entry = new Message(username, message);
            lock (syncLock)
            {
                if (store.Count == 15)
                {
                    store.RemoveAt(0);
                }

                store.Add(entry);
            }
        }

        public IEnumerable<Message> GetRecentMessages()
        {
            return store;
        }
    }
}