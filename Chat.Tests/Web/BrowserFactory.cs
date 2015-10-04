namespace Chat.Tests.Web
{
    using System;

    using Coypu;
    using Coypu.Drivers;

    public static class BrowserFactory
    {
        private static bool CI => bool.Parse(Environment.GetEnvironmentVariable("CI") ?? "False");

        public static BrowserSession Create()
        {
            var config = new SessionConfiguration
                             {
                                 AppHost = WebAppHosting.BaseUrl,
                                 Port = WebAppHosting.Port,
                                 Driver = typeof(Coypu.Drivers.Selenium.SeleniumWebDriver),
                                 Browser = CI ? Browser.PhantomJS : Browser.Chrome,
                                 Timeout = TimeSpan.FromSeconds(2),
                                 RetryInterval = TimeSpan.FromSeconds(0.2),
                                 WaitBeforeClick = TimeSpan.FromSeconds(0.5)
                             };

            return new BrowserSession(config);
        }
    }
}