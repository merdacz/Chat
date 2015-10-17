namespace Chat.Logic.Enhancers
{
    using System.Collections.Generic;
    using System.Linq;

    public class MessageProcessingPipeline : IMessageProcessingPipeline
    {
        private readonly IList<IMessageEnhancer> enhancers;

        public MessageProcessingPipeline(params IMessageEnhancer[] enhancers)
        {
            this.enhancers = enhancers.ToList();
        }

        public string Process(string message)
        {
            var context = new MessageProcessingContext(message);
            foreach (var enhancer in this.enhancers)
            {
                enhancer.Apply(context);
            }

            return context.CurrentMessage;
        }
    }
}