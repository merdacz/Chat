namespace Chat.Tests.Logic
{
    using System.Collections.Generic;

    using Chat.Logic;

    public class InMemoryMessageLog : IMessageLog
    {
        public static IList<Message> store = new List<Message>();

        public static object syncLock = new object();

        private readonly int recentMessagesLimit;

        public InMemoryMessageLog(int recentMessagesLimit)
        {
            this.recentMessagesLimit = recentMessagesLimit;
        }


        public void Save(string username, string message)
        {
            var entry = new Message(username, message);
            lock (syncLock)
            {
                if (store.Count == this.recentMessagesLimit)
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