namespace Chat.Tests.Logic
{
    using Chat.Logic;
    using Chat.Logic.Enhancers;

    using FluentAssertions;

    using Xunit;

    public class EnhancersTests
    {
        [Fact]
        public void Empty_pipeline_succeeds_without_changes()
        {
            var pipeline = new MessageProcessingPipeline();
            var input = A.RandomShortString();
            var output = pipeline.Process(input);
            output.Should().Be(input);
        }

        [Fact]
        public void Pipeline_runs_enhancers_in_configured_order()
        {
            var pipeline = new MessageProcessingPipeline(
                new PrependingEnhancer("1"),
                new PrependingEnhancer("2"),
                new PrependingEnhancer("3"));

            var output = pipeline.Process(string.Empty);

            output.Should().Be("321");
        }

        public class PrependingEnhancer : IMessageEnhancer
        {
            private readonly string prefix;

            public PrependingEnhancer(string prefix)
            {
                this.prefix = prefix;
            }

            public void Apply(MessageProcessingContext context)
            {
                var input = context.CurrentMessage;
                var output = this.prefix + input;
                context.CurrentMessage = output;
            }
        }
    }
}