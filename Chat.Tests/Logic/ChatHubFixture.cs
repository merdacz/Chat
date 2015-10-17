namespace Chat.Tests.Logic
{
    using System;

    using Chat.Logic;
    using Chat.Logic.Enhancers;
    using Chat.Logic.Log;
    using Chat.Web.Controllers;

    using Microsoft.AspNet.SignalR.Hubs;

    using Moq;

    public class ChatHubFixture : IChatHubAssertions
    {
        private ChatHub createdInstance = null;

        private IMessageLog messageLog = null;

        private IMessageProcessingPipeline messageProcessingPipeline = null;

        public IChatHubAssertions Checks => this;

        public ClientOperationsSpy Caller { get; private set; }

        public ClientOperationsSpy All { get; private set; }

        public ClientOperationsSpy Others { get; private set; }

        public int MaxCapcity => 4;

        public int MessageCountOnJoin => 4;

        private ChatHubFixture()
        {
            this.messageLog = new InMemoryMessageLog(this.MessageCountOnJoin);
            this.messageProcessingPipeline = new MessageProcessingPipeline();
        }

        public static ChatHubFixture Create()
        {
            return new ChatHubFixture();
        }

        public ChatHubFixture With(IMessageLog messageLog)
        {
            this.messageLog = messageLog;
            return this;
        }
        public ChatHubFixture With(IMessageProcessingPipeline pipeline)
        {
            this.messageProcessingPipeline = pipeline;
            return this;
        }

        public ChatHub CreateSut()
        {
            if (this.createdInstance != null)
            {
                throw new InvalidOperationException(
                    "Fixture supports creation only of a single createdInstance instance. ");
            }

            Chat chat = this.CreateChat();
            var hub = this.createdInstance = new ChatHub(chat);

            this.InitializeClients(hub);
            this.RegisterContext();

            return hub;
        }

        private Chat CreateChat()
        {
            var configuration = new Mock<IChatConfiguration>();
            configuration.Setup(x => x.GetMaxCapacity()).Returns(this.MaxCapcity);
            configuration.Setup(x => x.GetMessageCountOnJoin()).Returns(this.MessageCountOnJoin);

            var participants = new ParticipantsStore();
            var chat = new Chat(participants, configuration.Object, this.messageLog, this.messageProcessingPipeline);
            return chat;
        }

        private void InitializeClients(ChatHub hub)
        {
            var clients = new Mock<IHubCallerConnectionContext<dynamic>>();
            hub.Clients = clients.Object;
            this.Others = new ClientOperationsSpy();
            this.All = new ClientOperationsSpy();
            this.Caller = new ClientOperationsSpy();
            clients.Setup(x => x.Caller).Returns(this.Caller);
            clients.Setup(x => x.All).Returns(this.All);
            clients.Setup(x => x.Others).Returns(this.Others);
        }

        private void RegisterContext()
        {
            this.createdInstance.Context = new Mock<HubCallerContext>().Object;
            this.NewSession();
        }

        public void NewSession()
        {
            Mock.Get(this.createdInstance.Context)
                .SetupGet(x => x.ConnectionId).Returns(A.RandomShortString());
        }
    }
}