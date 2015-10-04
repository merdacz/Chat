namespace Chat.Logic
{
    using System.Collections.Generic;
    using System.Linq;

    using Raven.Client.Linq;

    public class RavenMessageLog : RavenDatabaseAware, IMessageLog
    {
        private readonly int recentMessagesLimit;

        public RavenMessageLog(IChatConfiguration configuration)
        {
            this.recentMessagesLimit = configuration.GetMessageCountOnJoin();
        }

        public void Save(string username, string message)
        {
            using (var session = this.OpenSession())
            {
                session.Store(new Message(username, message));
                session.SaveChanges();
            }
        }

        public IEnumerable<Message> GetRecentMessages()
        {
            using (var session = this.OpenSession())
            {
                var messages = session.Query<Message>()
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(15)
                    .ToList()
                    .Reverse<Message>();

                return messages;
            }
        }
    }
}