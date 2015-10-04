namespace Chat.Tests.Logic
{
    using System;

    using Chat.Logic;
    using Chat.Web.Controllers;

    using Microsoft.AspNet.SignalR.Hubs;

    using Moq;

    public class ChatHubFixture : IChatHubAssertions
    {
        private ChatHub createdInstance = null;

        public IChatHubAssertions Checks => this;

        public ClientOperationsSpy Caller { get; private set; }

        public ClientOperationsSpy All { get; private set; }

        public ClientOperationsSpy Others { get; private set; }

        public ChatHub CreateSut(IMessageLog messageLog = null)
        {
            if (this.createdInstance != null)
            {
                throw new InvalidOperationException("Fixture support creation only of a single createdInstance instance. ");
            }

            if (messageLog == null)
            {
                messageLog = new Mock<IMessageLog>().Object;
            }

            var configuration = new ChatConfiguration();
            var hub = new ChatHub(messageLog, configuration);
            var clients = new Mock<IHubCallerConnectionContext<dynamic>>();
            hub.Clients = clients.Object;
            this.Others = new ClientOperationsSpy();
            this.All = new ClientOperationsSpy();
            this.Caller = new ClientOperationsSpy();
            clients.Setup(x => x.Caller).Returns(this.Caller);
            clients.Setup(x => x.All).Returns(this.All);
            clients.Setup(x => x.Others).Returns(this.Others);

            var context = new Mock<HubCallerContext>();

            //// TODO temporary workaround to static createdInstance state
            ChatHub.ResetParticipants();
            context.SetupGet(x => x.ConnectionId).Returns(A.RandomShortString());
            hub.Context = context.Object;

            this.createdInstance = hub;
            return hub;
        }

        public void NewSession()
        {
            Mock.Get(this.createdInstance.Context).SetupGet(x => x.ConnectionId).Returns(A.RandomShortString());
        }
    }
}