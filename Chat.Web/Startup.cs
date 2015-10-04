namespace Chat.Web
{
    using System.Web.Mvc;

    using Chat.Logic;
    using Chat.Web.Controllers;

    using Microsoft.AspNet.SignalR;

    using Owin;
    public class Startup
    {
        private static readonly ParticipantsStore participantsStore = new ParticipantsStore();

        public void Configuration(IAppBuilder app)
        {
            GlobalHost.DependencyResolver.Register(
                typeof(ChatHub),
                () => new ChatHub(new Chat(participantsStore, new ChatConfiguration(), new InMemoryMessageLog())));
            app.MapSignalR();
        }
    }
}   