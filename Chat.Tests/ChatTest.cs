namespace Chat.Tests
{
    using System.Collections.Generic;

    using Chat.Logic;

    using FluentAssertions;
    using Xunit;

    public class ChatTest : IClassFixture<WebServerFixture>
    { 
        private readonly IChatConfiguration configuration = new ChatConfiguration();

        [Fact]
        public void Homepage_loads_correctly()
        {
            using (var alice = BrowserFactory.Create())
            {
                alice.Visit("/");
                alice.Title.Should().Contain("Welcome");
            }
        }

        [Fact]
        public void Cannot_join_once_chat_capacity_is_reached()
        {
            int maxChatCapacity = this.configuration.GetMaxCapacity();
            var chats = new List<ChatPageObject>();

            for (int i = 0; i < maxChatCapacity; i++)
            {
                var chat = new ChatPageObject(BrowserFactory.Create());
                chats.Add(chat);
                chat.Join($"User#{i}");
            }

            using (var aliceBrowser = BrowserFactory.Create())
            {
                var alice = new ChatPageObject(aliceBrowser);
                alice.Join("Alice Cooper");
                alice.Should().GetError("Chat is full");
            }

            chats.ForEach(
                x =>
                    {
                        x.Leave();
                        x.Session.Dispose();
                    });
        }

        [Fact]
        public void Pressing_Enter_will_send_message()
        {
            using (var aliceBrowser = BrowserFactory.Create())
            using (var bobBrowser = BrowserFactory.Create())
            {
                var alice = new ChatPageObject(aliceBrowser);
                var bob = new ChatPageObject(bobBrowser);

                alice.Join("Alice Cooper");
                bob.Join("Bob Dylan");
                alice.SendMessage("Hello Bob!");

                alice.Should().GetMessage("Hello Bob!");
                alice.Should().HaveMessageInputCleared();
                bob.Should().GetMessage("Hello Bob!");

                alice.Leave();
                bob.Leave();
            }
        }

        [Fact]
        public void Up_to_15​_last_messages_will_be_shown_upon_joining()
        {
            using (var aliceBrowser = BrowserFactory.Create())
            {
                var alice = new ChatPageObject(aliceBrowser);
                alice.Join("Alice Cooper");
                alice.SendMessage("Old message.");
                for (int num = 1; num <= 15; num++)
                {
                    alice.SendMessage($"Message #{num}");
                }

                alice.Leave();
            }

            using (var bobBrowser = BrowserFactory.Create())
            {
                var bob = new ChatPageObject(bobBrowser);
                bob.Join("Bob Dylan");

                bob.Should().NotGetMessage("Old message.");
                for (int num = 1; num <= 15; num++)
                {
                    bob.Should().GetMessage($"Message #{num}");
                }

                bob.Leave();
            }
        }

        [Fact]
        public void Screen_name_duplication_is_not_allowed()
        {
            using (var aliceBrowser = BrowserFactory.Create())
            using (var aliceImpostorBrowser = BrowserFactory.Create())
            {
                var alice = new ChatPageObject(aliceBrowser);
                alice.Join("Alice Cooper");

                var aliceImpostor = new ChatPageObject(aliceImpostorBrowser);
                aliceImpostor.Join("Alice Cooper");

                aliceImpostor.Should().GetError("Name already taken");

                alice.Leave();
            }
        }
    }
}
