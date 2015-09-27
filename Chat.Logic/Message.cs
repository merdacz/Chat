namespace Chat.Logic
{
    public class Message
    {
        public Message(string username, string text)
        {
            this.UserName = username;
            this.Text = text;
        }

        public string UserName { get; private set; }

        public string Text { get; private set; }
    }
}