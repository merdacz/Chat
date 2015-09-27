namespace Chat.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Coypu;

    using FluentAssertions;

    using OpenQA.Selenium;

    public class ChatPageObject : IChatPageAssertions
    {
        private readonly BrowserSession browser;

        public ChatPageObject(BrowserSession browser)
        {
            this.browser = browser;
            this.browser.Visit("/");
        }

        private ElementScope MessageBox => this.browser.FindId("msg");

        private ElementScope UserBox => this.browser.FindId("usr");

        private ElementScope JoinButton => this.browser.FindId("join");

        private ElementScope LeaveButton => this.browser.FindId("leave");

        private string ErrorAlert => this.browser.FindId("error", Options.Invisible).Text;

        private IEnumerable<string> Messages
        {
            get
            {
                return this.browser.FindAllCss("#messages>li").Select(x => x.Text);
            }
        }

        public BrowserSession Session => this.browser;

        public ChatPageObject Join(string userName)
        {
            this.UserBox.FillInWith(userName);
            this.JoinButton.Click(); 
            return this;
        }

        public ChatPageObject SendMessage(string message)
        {
            this.MessageBox.FillInWith(message);
            this.MessageBox.SendKeys(Keys.Enter);
            Thread.Sleep(500);
            return this;
        }

        public ChatPageObject Leave()
        {
            this.LeaveButton.Click();
            return this;
        }

        public IChatPageAssertions Should()
        {
            return this;
        }

        void IChatPageAssertions.GetMessage(string message)
        {
            this.Messages.Should().Contain(msg => msg.Contains(message));
        }

        void IChatPageAssertions.HaveMessageInputCleared()
        {
            this.MessageBox.Text.Should().BeEmpty();
        }

        void IChatPageAssertions.NotGetMessage(string message)
        {
            this.Messages.Should().NotContain(msg => msg.Contains(message));
        }

        void IChatPageAssertions.GetError(string message)
        {
            this.ErrorAlert.Contains(message);
        }
    }
}