﻿namespace Chat.Web
{
    using System;
    using System.Web.Mvc;

    using Chat.Logic;
    using Chat.Web.Controllers;

    using Microsoft.AspNet.SignalR;

    using Owin;
    public class Startup
    {
        private static readonly ParticipantsStore participantsStore = new ParticipantsStore();

        private static bool UnderTest => bool.Parse(Environment.GetEnvironmentVariable("UnderTest") ?? "False");

        public void Configuration(IAppBuilder app)
        {
            GlobalHost.DependencyResolver.Register(
                typeof(ChatHub),
                () =>
                    {
                        var configuration = new ChatConfiguration();
                        IMessageLog messageLog = null;
                        if (UnderTest)
                        {
                            messageLog = new InMemoryMessageLog(configuration);
                        }
                        else
                        {
                            messageLog = new RavenMessageLog(configuration);
                        }

                        var chat = new Chat(
                            participantsStore,
                            configuration,
                            messageLog);
                        return new ChatHub(chat);
                    });
            app.MapSignalR();
        }
    }
}   