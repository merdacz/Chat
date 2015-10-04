namespace Chat.Tests.Logic
{
    using Chat.Logic;

    using FluentAssertions;

    using Xunit;

    public class LogicTests
    {
        [Fact]
        public void Chat_joining()
        {
            var fixture = ChatHubFixture.Create();
            var sut = fixture.CreateSut();
            var alice = A.RandomShortString();
            sut.Join(alice);

            var check = fixture.Checks;
            check.Others.LastUserJoined.Should().Be(alice);
        }

        [Fact]
        public void Chat_leaving()
        {
            var fixture = ChatHubFixture.Create();
            var sut = fixture.CreateSut();
            var alice = A.RandomShortString();
            sut.Join(alice);
            sut.Leave();

            var check = fixture.Checks;
            check.All.LastUserLeft.Should().Be(alice);
        }

        [Fact]
        public void Leaving_without_joining_will_fail()
        {
            var fixture = ChatHubFixture.Create();
            var sut = fixture.CreateSut();
            sut.Leave();

            var check = fixture.Checks;
            check.Caller.LastInvalidMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Sending_message_before_joining_is_not_allowed()
        {
            var fixture = ChatHubFixture.Create();
            var sut = fixture.CreateSut();
            sut.SendMessage("Hello everybody!");
            var check = fixture.Checks;
            check.Caller.LastInvalidMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Once_joined_user_can_send_messages()
        {
            var fixture = ChatHubFixture.Create();
            var sut = fixture.CreateSut();
            var alice = A.RandomShortString();
            sut.Join(alice);
            sut.SendMessage("Hello everybody!");
            var check = fixture.Checks;
            check.All.LastMessage.UserName.Should().Be(alice);
            check.All.LastMessage.Text.Should().Be("Hello everybody!");
        }

        [Fact]
        public void Duplicated_username_wont_work()
        {
            var fixture = ChatHubFixture.Create();
            var sut = fixture.CreateSut();
            var alice = A.RandomShortString();

            fixture.NewSession();
            sut.Join(alice);
            fixture.NewSession();
            sut.Join(alice);

            var check = fixture.Checks;
            check.Caller.LastInvalidMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Need_to_leave_room_before_joining_again()
        {
            var fixture = ChatHubFixture.Create();
            var sut = fixture.CreateSut();
            sut.Join("Alice Cooper");
            sut.Join("Bob Dylan");
            var check = fixture.Checks;
            check.Caller.LastInvalidMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Gets_messages_upon_joining_chat()
        {
            var fixture = ChatHubFixture.Create().With(new InMemoryMessageLog());
            var sut = fixture.CreateSut();
            sut.Join(A.RandomShortString());
            sut.SendMessage("Old message. ");
            for (int i = 1; i <= 15; i++)
            {
                sut.SendMessage($"Message #{i}. ");
            }

            fixture.NewSession();
            sut.Join(A.RandomShortString());
            var check = fixture.Checks;
            check.Caller.LastMessagesUponJoin.Should().HaveCount(15);
        }

        [Fact]
        public void Cannot_join_once_limit_hit()
        {
            var fixture = ChatHubFixture.Create();
            var sut = fixture.CreateSut();
            for (int i = 0; i <= fixture.MaxCapcity; i++)
            {
                fixture.NewSession();
                sut.Join(A.RandomShortString());
            }

            var check = fixture.Checks;
            check.Caller.LastInvalidMessage.Should().Contain("full");
        }
    }
}