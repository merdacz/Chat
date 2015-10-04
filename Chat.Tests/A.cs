namespace Chat.Tests
{
    using System.IO;

    public static class A
    {
        public static string RandomShortString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", "");
            return path;
        }
    }
}