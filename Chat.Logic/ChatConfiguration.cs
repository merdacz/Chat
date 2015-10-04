namespace Chat.Logic
{
    using System;
    using System.Configuration;

    public class ChatConfiguration : IChatConfiguration
    {
        public int GetMaxCapacity()
        {
            return int.Parse(ConfigurationManager.AppSettings["Chat.MaxCapacity"]); 
        }
    }
}