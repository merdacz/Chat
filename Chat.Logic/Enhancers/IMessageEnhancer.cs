namespace Chat.Logic.Enhancers
{
    public interface IMessageEnhancer
    {
        void Apply(MessageProcessingContext context);
    }
}