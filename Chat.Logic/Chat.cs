namespace Chat.Logic
{
    using System.Linq;

    public class Chat : IChat
    {
        private readonly IChatConfiguration configuration;

        private readonly IMessageLog messageLog;

        public readonly ParticipantsStore participants;

        public Chat(ParticipantsStore participants, IChatConfiguration configuration, IMessageLog messageLog)
        {
            this.participants = participants;
            this.configuration = configuration;
            this.messageLog = messageLog;
        }

        public ChatSnapshot TakeSnapshot()
        {
            var result = new ChatSnapshot();
            result.Participants = this.participants.Values;
            result.RecentMessages = this.messageLog.GetRecentMessages();
            return result;
        }

        public Result Join(string username, string connectionId)
        {
            if (this.HasJoined(connectionId))
            {
                return Result.Error(
                    $"Already logged in as {this.participants[connectionId]}, please leave chat first. ");
            }

            if (this.participants.Values.Contains(username))
            {
                return Result.Error("Name already taken, please choose another one. ");
            }

            if (this.participants.Count >= this.configuration.GetMaxCapacity())
            {
                return Result.Error("Chat is full, please retry. ");
            }

            this.participants.Add(connectionId, username);
            return Result.OK(username);
        }

        public Result Leave(string connectionId)
        {
            if (!this.participants.ContainsKey(connectionId))
            {
                return Result.Error("Already left the chat. ");
            }
            var username = this.participants[connectionId];
            this.participants.Remove(connectionId);
            return Result.OK(username);
        }

        public bool HasJoined(string connectionId)
        {
            return this.participants.ContainsKey(connectionId);
        }

        public Result SendMessage(string connectionId, string message)
        {
            if (!this.HasJoined(connectionId))
            {
                return Result.Error("Cannot send messages until joined. ");
            }

            string username = this.participants[connectionId];
            this.messageLog.Save(username, message);
            return Result.OK(username);
        }
    }
}