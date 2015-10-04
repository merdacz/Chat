namespace Chat.Tests.Logic
{
    using Chat.Logic;
    using Chat.Web.Controllers;

    using FluentAssertions;

    using Xunit;

    public class LogicTests
    {
        [Fact]
        public void Chat_joining()
        {
            var fixture = new ChatHubFixture();
            var hub = fixture.CreateSut();
            var alice = A.RandomShortString();
            hub.Join(alice);

            var check = fixture.Checks;
            check.Others.LastUserJoined.Should().Be(alice);
        }

        [Fact]
        public void Chat_leaving()
        {
            var fixture = new ChatHubFixture();
            var hub = fixture.CreateSut();
            var alice = A.RandomShortString();
            hub.Join(alice);
            hub.Leave();

            var check = fixture.Checks;
            check.All.LastUserLeft.Should().Be(alice);
        }

        [Fact]
        public void Leaving_without_joining_will_fail()
        {
            var fixture = new ChatHubFixture();
            var hub = fixture.CreateSut();
            hub.Leave();

            var check = fixture.Checks;
            check.Caller.LastInvalidMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Sending_message_before_joining_is_not_allowed()
        {
            var fixture = new ChatHubFixture();
            var hub = fixture.CreateSut();
            hub.SendMessage("Hello everybody!");
            var check = fixture.Checks;
            check.Caller.LastInvalidMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Once_joined_user_can_send_messages()
        {
            var fixture = new ChatHubFixture();
            var hub = fixture.CreateSut();
            var alice = A.RandomShortString();
            hub.Join(alice);
            hub.SendMessage("Hello everybody!");
            var check = fixture.Checks;
            check.All.LastMessage.UserName.Should().Be(alice);
            check.All.LastMessage.Text.Should().Be("Hello everybody!");
        }

        [Fact]
        public void Duplicated_username_wont_work()
        {
            var fixture = new ChatHubFixture();
            var hub = fixture.CreateSut();
            var alice = A.RandomShortString();

            fixture.NewSession();
            hub.Join(alice);
            fixture.NewSession();
            hub.Join(alice);

            var check = fixture.Checks;
            check.Caller.LastInvalidMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Need_to_leave_room_before_joining_again()
        {
            var fixture = new ChatHubFixture();
            var hub = fixture.CreateSut();
            hub.Join("Alice Cooper");
            hub.Join("Bob Dylan");
            var check = fixture.Checks;
            check.Caller.LastInvalidMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Gets_messages_upon_joining_chat()
        {
            var fixture = new ChatHubFixture();
            var hub = fixture.CreateSut(new InMemoryMessageLog());
            hub.Join(A.RandomShortString());
            hub.SendMessage("Old message. ");
            for (int i = 1; i <= 15; i++)
            {
                hub.SendMessage($"Message #{i}. ");
            }

            fixture.NewSession();
            hub.Join(A.RandomShortString());
            var check = fixture.Checks;
            check.Caller.LastMessagesUponJoin.Should().HaveCount(15);
        }

        [Fact]
        public void Cannot_join_once_limit_hit()
        {
            var fixture = new ChatHubFixture();
            var hub = fixture.CreateSut();
            var max = new ChatConfiguration().GetMaxCapacity();
            for (int i = 0; i <= max; i++)
            {
                fixture.NewSession();
                hub.Join(A.RandomShortString());
            }

            var check = fixture.Checks;
            check.Caller.LastInvalidMessage.Should().Contain("full");
        }
    }
}