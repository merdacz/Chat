namespace Chat.Tests
{
    using System.IO;

    public static class WebAppHosting
    {
        public static string Path
        {
            get
            {
                var current = new DirectoryInfo(Directory.GetCurrentDirectory());
                var solution = current.Parent.Parent.Parent;
                return solution.FullName + @"\Chat.Web";
            }
        }

        public static int Port => 12345;

        public static string BaseUrl => "localhost";
    }
}