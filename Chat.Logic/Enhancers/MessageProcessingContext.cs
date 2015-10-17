namespace Chat.Logic.Enhancers
{
    public class MessageProcessingContext
    {
        private readonly string initialMessage;

        public string CurrentMessage { get; set; }

        public MessageProcessingContext(string initialMessage)
        {
            this.initialMessage = initialMessage;
            this.CurrentMessage = initialMessage;
        }
    }
}