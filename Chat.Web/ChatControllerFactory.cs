namespace Chat.Web
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Chat.Logic;
    using Chat.Web.Controllers;

    public class ChatControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null) return null;

            if (controllerType == typeof(ChatController))
            {
                return new ChatController(new InMemoryMessageLog());
            }

            return base.GetControllerInstance(requestContext, controllerType);
        }
    }
}