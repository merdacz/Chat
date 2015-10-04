namespace Chat.Tests.Web
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