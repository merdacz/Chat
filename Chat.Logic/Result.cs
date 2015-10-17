namespace Chat.Logic
{
    public class Result
    {
        private Result(bool success, string errorMessage = null)
        {
            this.Success = success;
            this.ErrorMessage = errorMessage;
        }

        public bool Success { get; private set; }

        public string ErrorMessage { get; private set; }

        public string Username { get; set; }

        public string ProcessedMessage { get; set; }

        public static Result OK(string username)
        {
            var result = new Result(true);
            result.Username = username;
            return result;
        }

        public static Result OK(string username, string processedMessage)
        {
            var result = OK(username);
            result.ProcessedMessage = processedMessage;
            return result;
        }

        public static Result Error(string message)
        {
            return new Result(false, message);
        }
    }
}