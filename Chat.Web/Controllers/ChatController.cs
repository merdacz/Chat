namespace Chat.Web.Controllers
{
    using System.Web.Mvc;

    using Chat.Logic;

    public class ChatController : Controller
    {
        public ActionResult Index()
        {
            return this.View();
        }
    }
}