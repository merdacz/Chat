namespace Chat.Logic
{
    using System;

    public class Message
    {
        public Message(string username, string text)
        {
            this.UserName = username;
            this.Text = text;
            this.CreatedAt = DateTimeOffset.UtcNow;
        }

        public DateTimeOffset CreatedAt { get; private set; }

        public string UserName { get; private set; }

        public string Text { get; private set; }
    }
}