namespace Chat.Web.Controllers
{
    using System.Web.Mvc;

    using Chat.Logic;

    public class ChatController : Controller
    {
        private readonly IMessageLog messageLog;

        public ChatController(IMessageLog messageLog)
        {
            this.messageLog = messageLog;
        }

        public ActionResult Index()
        {
            var messages = this.messageLog.GetRecentMessages();
            return this.View(messages);
        }
    }
}