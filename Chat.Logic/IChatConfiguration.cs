namespace Chat.Logic
{
    public interface IChatConfiguration
    {
        int GetMaxCapacity();

        int GetMessageCountOnJoin();
    }
}