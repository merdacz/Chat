namespace Chat.Tests.Web
{
    using System;

    using Coypu;
    using Coypu.Drivers;

    public static class BrowserFactory
    {
        public static BrowserSession Create()
        {
            var config = new SessionConfiguration
                             {
                                 AppHost = WebAppHosting.BaseUrl,
                                 Port = WebAppHosting.Port,
                                 Driver = typeof(Coypu.Drivers.Selenium.SeleniumWebDriver),
                                 Browser = Browser.Chrome,
                                 Timeout = TimeSpan.FromSeconds(1),
                                 RetryInterval = TimeSpan.FromSeconds(0.1),
                                 WaitBeforeClick = TimeSpan.FromSeconds(0.5)
                             };

            return new BrowserSession(config);
        }
    }
}