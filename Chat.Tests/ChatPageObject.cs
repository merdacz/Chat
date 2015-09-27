namespace Chat.Tests
{
    using System.Collections.Generic;

    using Coypu;

    using FluentAssertions;

    using OpenQA.Selenium;

    public class ChatPageObject
    {
        private readonly BrowserSession browser;

        public ChatPageObject(BrowserSession browser)
        {
            this.browser = browser;
            this.browser.Visit("/");
        }

        public string Input => this.browser.FindId("msg").Value;

        public IEnumerable<SnapshotElementScope> Messages => this.browser.FindAllCss("#messages>li");

        public ChatPageObject SendMessage(string message)
        {
            this.browser.FillIn("msg").With(message);
            this.browser.FindId("msg").SendKeys(Keys.Enter);
            return this;
        }

        public void GotMessage(string message)
        {
            this.Messages.Should().Contain(msg => msg.Text.Contains(message));
        }

        public void InputWasCleared()
        {
            this.Input.Should().BeEmpty();
        }
    }
}