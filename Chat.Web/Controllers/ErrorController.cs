namespace Chat.Web.Controllers
{
    using System.Web.Mvc;

    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            return this.View();
        }

        public ActionResult NotFound()
        {
            return this.View("NotFound");
        }
    }
}