namespace Chat.Logic
{
    public interface IChat
    {
        Result Join(string username, string connectionId);

        Result Leave(string connectionId);

        bool HasJoined(string connectionId);

        Result SendMessage(string connectionId, string message);

        ChatSnapshot TakeSnapshot();
    }
}