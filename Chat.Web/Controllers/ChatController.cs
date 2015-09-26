using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chat.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNet.SignalR;

    public class ChatController : Controller
    {
        // GET: Chat
        public ActionResult Index()
        {
            return View();
        }
    }

    public class ChatHub : Hub
    {
        public void SendMessage(string message)
        {
            this.Clients.All.newMessage(this.Context.ConnectionId, message);
        }
    }
}