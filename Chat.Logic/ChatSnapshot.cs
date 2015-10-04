namespace Chat.Logic
{
    using System.Collections.Generic;

    public class ChatSnapshot
    {
        public IEnumerable<string> Participants { get; set; }

        public IEnumerable<Message> RecentMessages { get; set; }
    }
}