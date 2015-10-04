namespace Chat.Web.Controllers
{
    using System.Web.Mvc;

    public class ChatController : Controller
    {
        public ActionResult Index()
        {
            return this.View();
        }
    }
}