namespace Chat.Web
{
    using System.Diagnostics;

    using log4net;

    using Microsoft.AspNet.SignalR.Hubs;

    using IExceptionFilter = System.Web.Mvc.IExceptionFilter;

    public class SignalrErrorHandler : HubPipelineModule
    {
        private readonly ILog log = LogManager.GetLogger(typeof(SignalrErrorHandler));

        protected override void OnIncomingError(ExceptionContext exceptionContext, IHubIncomingInvokerContext invokerContext)
        {
            this.log.Error("Error during SignalR processing. ", exceptionContext.Error);
        }
    }

    public class ExceptionHandlingAttribute : IExceptionFilter
    {
        private readonly ILog log = LogManager.GetLogger(typeof(ExceptionHandlingAttribute));

        public void OnException(System.Web.Mvc.ExceptionContext filterContext)
        {
            this.log.Error("Error during regular processing. ", filterContext.Exception);
        }
    }
}