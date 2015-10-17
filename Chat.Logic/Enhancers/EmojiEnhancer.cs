namespace Chat.Logic.Enhancers
{
    using System.Text;
    using System.Web;

    using EmojiSharp;

    public class EmojiEnhancer : IMessageEnhancer
    {
        public void Apply(MessageProcessingContext context)
        {
            var input = context.CurrentMessage;
            var output = new StringBuilder(input);
            foreach (var emoji in Emoji.All.Values)
            {
                foreach (var text in emoji.Texts)
                {
                    var encodedText = HttpUtility.HtmlEncode(text);
                    var entity = "&#x" + emoji.Unified + ";";
                    output.Replace(text, entity);
                    if (encodedText != text)
                    {
                        output.Replace(encodedText, entity);
                    }
                }
            }

            context.CurrentMessage = output.ToString();
        }
    }
}