namespace Chat.Logic.Enhancers
{
    using System.Web;

    public class EncodingEnhancer : IMessageEnhancer
    {
        public void Apply(MessageProcessingContext context)
        {
            var input = context.CurrentMessage;
            var output = HttpUtility.HtmlEncode(input);
            context.CurrentMessage = output;
        }
    }
}