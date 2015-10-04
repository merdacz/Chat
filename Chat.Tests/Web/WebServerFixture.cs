namespace Chat.Tests.Web
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    public class WebServerFixture : IDisposable
    {
        private Process iisExpress;

        public WebServerFixture()
        {
            Environment.SetEnvironmentVariable("UnderTest", "True");
            var thread = new Thread(this.StartIisExpress) { IsBackground = true };
            thread.Start();
        }

        public void Dispose()
        {
            this.SafelyKill(this.iisExpress);
        }

        private void StartIisExpress()
        {
            var startInfo = new ProcessStartInfo
                                {
                                    WindowStyle = ProcessWindowStyle.Normal,
                                    ErrorDialog = true,
                                    LoadUserProfile = true,
                                    CreateNoWindow = false,
                                    UseShellExecute = false,
                                    Arguments =
                                        $"/path:\"{WebAppHosting.Path}\" /port:{WebAppHosting.Port}"
                                };

            var programfiles = string.IsNullOrEmpty(startInfo.EnvironmentVariables["programfiles"])
                                   ? startInfo.EnvironmentVariables["programfiles(x86)"]
                                   : startInfo.EnvironmentVariables["programfiles"];

            startInfo.FileName = programfiles + "\\IIS Express\\iisexpress.exe";

            try
            {
                this.iisExpress = new Process { StartInfo = startInfo };

                this.iisExpress.Start();
                this.iisExpress.WaitForExit();
            }
            catch
            {
                this.SafelyKill(this.iisExpress);
            }
        }

        private void SafelyKill(Process process)
        {
            try
            {
                process.Kill();
            }
            catch (Exception)
            {
                // ignore we are shutting down already
            }
        }
    }
}