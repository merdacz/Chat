namespace Chat.Tests
{
    using FluentAssertions;
    using Xunit;

    public class ChatTest : IClassFixture<WebServerFixture>
    { 
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
        public void Pressing_Enter_will_send_message()
        {
            using (var aliceBrowser = BrowserFactory.Create())
            using (var bobBrowser = BrowserFactory.Create())
            {
                var alice = new ChatPageObject(aliceBrowser);
                var bob = new ChatPageObject(bobBrowser);

                alice.SendMessage("Hello Bob!");

                alice.GotMessage("Hello Bob!");
                alice.InputWasCleared();
                bob.GotMessage("Hello Bob!");
            }
        }

        [Fact(Skip = "Not implemented yet.")]
        public void When_the_user_logs_in__the_last_15​_messages_will_be_shown()
        {
            using (var aliceBrowser = BrowserFactory.Create())
            {
                var alice = new ChatPageObject(aliceBrowser);
                for (int num = 1; num <= 15; num++)
                {
                    alice.SendMessage($"Message #{num}");
                }
            }

            using (var bobBrowser = BrowserFactory.Create())
            {
                var bob = new ChatPageObject(bobBrowser);
                bob.Messages.Should().HaveCount(15);
            }
        }
    }
}
