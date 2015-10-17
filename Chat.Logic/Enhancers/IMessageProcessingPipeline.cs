namespace Chat.Logic.Enhancers
{
    public interface IMessageProcessingPipeline
    {
        string Process(string message);
    }
}