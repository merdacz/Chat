namespace Chat.Tests
{
    using System.Collections.Generic;

    using Chat.Logic;
    using Chat.Web.Controllers;

    using FluentAssertions;

    using Microsoft.AspNet.SignalR.Hubs;

    using Moq;

    using Xunit;

    public class LogicTests
    {
        [Fact]
        public void Chat_joining()
        {
            ChatHub hub = this.CreateSut();
            var alice = A.RandomShortString();
            hub.Join(alice);
            this.Others.LastUserJoined.Should().Be(alice);
        }

        [Fact]
        public void Chat_leaving()
        {
            ChatHub hub = this.CreateSut();
            var alice = A.RandomShortString();
            hub.Join(alice);
            hub.Leave();
            this.All.LastUserLeft.Should().Be(alice);
        }

        [Fact]
        public void Leaving_without_joining_will_fail()
        {
            ChatHub hub = this.CreateSut();
            hub.Leave();
            this.Caller.LastInvalidMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Sending_message_before_joining_is_not_allowed()
        {
            ChatHub hub = this.CreateSut();
            hub.SendMessage("Hello everybody!");
            this.Caller.LastInvalidMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Once_joined_user_can_send_messages()
        {
            ChatHub hub = this.CreateSut();
            var alice = A.RandomShortString();
            hub.Join(alice);
            hub.SendMessage("Hello everybody!");
            this.All.LastMessage.UserName.Should().Be(alice);
            this.All.LastMessage.Text.Should().Be("Hello everybody!");
        }

        [Fact]
        public void Duplicated_username_wont_work()
        {
            ChatHub hub = this.CreateSut();
            var alice = A.RandomShortString();

            NewSession(hub);
            hub.Join(alice);
            NewSession(hub);
            hub.Join(alice);

            this.Caller.LastInvalidMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Need_to_leave_room_before_joining_again()
        {
            ChatHub hub = this.CreateSut();
            hub.Join("Alice Cooper");
            hub.Join("Bob Dylan");
            this.Caller.LastInvalidMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Gets_messages_upon_joining_chat()
        {
            ChatHub hub = this.CreateSut(new InMemoryMessageLog());
            hub.Join(A.RandomShortString());
            hub.SendMessage("Old message. ");
            for (int i = 1; i <= 15; i++)
            {
                hub.SendMessage($"Message #{i}. ");
            }

            NewSession(hub);
            hub.Join(A.RandomShortString());
            this.Caller.LastMessagesUponJoin.Should().HaveCount(15);
        }

        [Fact]
        public void Cannot_join_once_limit_hit()
        {
            ChatHub hub = this.CreateSut();
            var max = new ChatConfiguration().GetMaxCapacity();
            for (int i = 0; i <= max; i++)
            {
                NewSession(hub);
                hub.Join(A.RandomShortString());
            }

            this.Caller.LastInvalidMessage.Should().Contain("full");
        }

        private static void NewSession(ChatHub hub)
        {
            Mock.Get(hub.Context).SetupGet(x => x.ConnectionId).Returns(A.RandomShortString());
        }

        private ChatHub CreateSut(IMessageLog messageLog = null)
        {
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

            //// TODO temporary workaround to static hub state
            ChatHub.ResetParticipants(); 
            context.SetupGet(x => x.ConnectionId).Returns(A.RandomShortString());
            hub.Context = context.Object;

            return hub;
        }

        public ClientOperationsSpy Caller { get; set; }

        public ClientOperationsSpy All { get; set; }

        public ClientOperationsSpy Others { get; set; }

        public interface IClientOperations
        {
            void invalidOperation(string errorMessage);

            void joinedSuccessfully(IEnumerable<string> userList, IEnumerable<Message> recentMessages);

            void leftSuccessfully();

            void newUserJoined(string username);

            void userLeft(string username);

            void newMessage(string username, string message);
        }

        public class ClientOperationsSpy : IClientOperations {
            public string LastUserJoined { get; private set; }

            public string LastInvalidMessage { get; private set; }

            public Message LastMessage { get; private set; }

            public string LastUserLeft { get; private set; }

            public bool LeftSuccessfully { get; private set; }

            public bool JoinedSuccessfully { get; private set; }
            public IList<string> LastUsersUponJoin { get; private set; }

            public IList<Message> LastMessagesUponJoin{ get; set; }

            public void invalidOperation(string errorMessage)
            {
                this.LastInvalidMessage = errorMessage;
            }

            public void joinedSuccessfully(IEnumerable<string> userList, IEnumerable<Message> recentMessages)
            {
                this.JoinedSuccessfully = true;
                this.LeftSuccessfully = false;
                this.LastUsersUponJoin = new List<string>(userList);
                this.LastMessagesUponJoin = new List<Message>(recentMessages);
            }

            public void leftSuccessfully()
            {
                this.JoinedSuccessfully = false;
                this.LeftSuccessfully = true;
            }

            public void newUserJoined(string username)
            {
                this.LastUserJoined = username;
            }

            public void userLeft(string username)
            {
                this.LastUserLeft = username;
            }

            public void newMessage(string username, string text)
            {
                var message = new Message(username, text);
                this.LastMessage = message;
            }
        }
    }
}