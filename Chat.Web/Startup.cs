namespace Chat.Web
{
    using Chat.Web.Controllers;

    using Owin;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}   