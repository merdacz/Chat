namespace Chat.Tests.Logic
{
    public interface IChatHubAssertions
    {
        ClientOperationsSpy Caller { get; }

        ClientOperationsSpy All { get; }

        ClientOperationsSpy Others { get; }
    }
}