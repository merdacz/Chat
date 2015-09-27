using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Logic
{
    public class ChatConfiguration : IChatConfiguration
    {
        private static bool UnderTest => bool.Parse(Environment.GetEnvironmentVariable("UnderTest") ?? "False");

        public int GetMaxCapacity()
        {
            return UnderTest ? 3 : 20;
        }
    }
}
