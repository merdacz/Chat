﻿namespace Chat.Tests
{
    using System.Collections.Generic;

    using Chat.Logic;

    using Coypu.Matchers;

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
                chat.Join();
            }

            using (var aliceBrowser = BrowserFactory.Create())
            {
                var alice = new ChatPageObject(aliceBrowser);
                alice.Join();
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

                alice.Join();
                bob.Join();
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
                alice.Join();
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
                bob.Join();

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

        [Fact]
        public void Message_gets_encoded_before_display()
        {
            using (var aliceBrowser = BrowserFactory.Create())
            using (var bobBrowser = BrowserFactory.Create())
            {
                var alice = new ChatPageObject(aliceBrowser);
                var bob = new ChatPageObject(bobBrowser);

                alice.Join();
                bob.Join();
                alice.SendMessage("Beware Bob! I want to <script>alert('hack you!');</script>");
                
                // checking against dialog would be problematic in phantomjs.
                // since it does not support alert directly webdriver's hasdialog won't work.
                // instead we just explicitly validate encoded version appears on screen.
                bob.Should().GetMessage("Beware Bob! I want to <script>alert('hack you!');</script>");

                alice.Leave();
                bob.Leave();
            }
        }

        [Fact]
        public void User_names_gets_encoded_before_display()
        {
            using (var aliceBrowser = BrowserFactory.Create())
            using (var bobBrowser = BrowserFactory.Create())
            {
                var alice = new ChatPageObject(aliceBrowser);
                var bob = new ChatPageObject(bobBrowser);

                alice.Join();
                bob.Join("<script>alert('Hacker');</script>");

                // checking against dialog would be problematic in phantomjs.
                // since it does not support alert directly webdriver's hasdialog won't work.
                // instead we just explicitly validate encoded version appears on screen.
                alice.Should().See("<script>alert('Hacker');</script>");
                alice.Should().GetMessage("<script>alert('Hacker');");
                bob.Should().GetMessage("<script>alert('Hacker');");

                alice.Leave();
                bob.Leave();
            }
        }
    }
}
