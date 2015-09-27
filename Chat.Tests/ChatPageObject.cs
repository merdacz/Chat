namespace Chat.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

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

        public IEnumerable<string> Messages
        {
            get
            {
                return this.browser.FindAllCss("#messages>li").Select(x => x.Text);
            }
        }

        public ChatPageObject Join(string userName)
        {
            this.browser.FillIn("usr").With(userName);
            this.browser.ClickButton("join");
            return this;
        }

        public ChatPageObject SendMessage(string message)
        {
            this.browser.FillIn("msg").With(message);
            this.browser.FindId("msg").SendKeys(Keys.Enter);
            Thread.Sleep(500);
            return this;
        }

        public ChatPageObject Leave()
        {
            this.browser.ClickButton("leave");
            return this;
        }

        public void GotMessage(string message)
        {
            this.Messages.Should().Contain(msg => msg.Contains(message));
        }

        public void InputWasCleared()
        {
            this.Input.Should().BeEmpty();
        }

        public void DidNotGetMessage(string message)
        {
            this.Messages.Should().NotContain(msg => msg.Contains(message));
        }
    }
}