namespace Chat.Logic.Log
{
    using System.Collections.Generic;

    public class InMemoryMessageLog : IMessageLog
    {
        public static IList<Message> store = new List<Message>();

        public static object syncLock = new object();

        private readonly int recentMessagesLimit;

        public InMemoryMessageLog(IChatConfiguration configuration)
        {
            this.recentMessagesLimit = configuration.GetMessageCountOnJoin();
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