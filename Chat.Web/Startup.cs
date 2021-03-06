﻿namespace Chat.Web
{
    using System;
    using System.Web.Mvc;

    using Chat.Logic;
    using Chat.Logic.Enhancers;
    using Chat.Logic.Log;
    using Chat.Web.Controllers;

    using log4net;

    using Microsoft.AspNet.SignalR;

    using Owin;
    public class Startup
    {
        private static readonly ParticipantsStore participantsStore = new ParticipantsStore();

        private static bool UnderTest => bool.Parse(Environment.GetEnvironmentVariable("UnderTest") ?? "False");

        public void Configuration(IAppBuilder app)
        {
            GlobalHost.HubPipeline.AddModule(new SignalrErrorHandler());
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
                            // new RavenMessageLog(configuration) based on embeded
                            // Raven loads much too long, we will use in memory till
                            // the issues is resolved (via external Raven or switch to another
                            // storage provider);
                            messageLog = new InMemoryMessageLog(configuration);
                        }

                        var messageProcessor = new MessageProcessingPipeline(
                            new EncodingEnhancer(),
                            new EmojiEnhancer());

                        var chat = new Chat(
                            participantsStore,
                            configuration,
                            messageLog,
                            messageProcessor);
                        return new ChatHub(chat);
                    });
            app.MapSignalR();
        }
    }
}   