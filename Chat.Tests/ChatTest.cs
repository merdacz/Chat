using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Tests
{
    using System;
    using System.Threading;

    using Coypu;

    using FluentAssertions;

    using Xunit;

    public class ChatTest : IClassFixture<WebServerFixture>
    { 
        [Fact]
        public void HomePageLoadsCorrectly()
        {
            using (var browser = BrowserFactory.Create())
            {
                browser.Visit("/");
                browser.Title.Should().Contain("Welcome");
            }
        }

        [Fact]
        public void ChatMessagesGetBroadcasted()
        {
            using (var alice = BrowserFactory.Create())
            using (var bob = BrowserFactory.Create())
            {
                alice.Visit("/");
                bob.Visit("/");
                alice.FillIn("msg").With("Hello Bob!");
                alice.ClickButton("broadcast");

                alice.FindId("messages").HasContent("Hello Bob!").Should().BeTrue();
                bob.FindId("messages").HasContent("Hello Bob!").Should().BeTrue();
            }
        } 
    }
}
