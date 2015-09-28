namespace Chat.Tests
{
    public interface IChatPageAssertions
    {
        void GetMessage(string message);

        void HaveMessageInputCleared();

        void NotGetMessage(string message);

        void GetError(string message);

        void See(string username);
    }
}