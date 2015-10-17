namespace Chat.Tests.Logic
{
    using System.Web;
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

        [Theory]
        [InlineData(":)")]
        [InlineData("some :) data around")]
        [InlineData("Multiple emoticons :> and :)")]
        public void Emoji_enhancer_updates_eligible_input(string input)
        {
            var context = new MessageProcessingContext(input);
            var sut = new EmojiEnhancer();
            sut.Apply(context);

            context.CurrentMessage.Should().NotBe(input);
        }

        [Theory]
        [InlineData(": )")]
        [InlineData("some plain text to emoji")]
        [InlineData("")]
        public void Emoji_enhancer_keeps_non_eligible_input(string input)
        {
            var context = new MessageProcessingContext(input);
            var sut = new EmojiEnhancer();
            sut.Apply(context);

            context.CurrentMessage.Should().Be(input);
        }

        [Fact]
        public void Emoji_enhancer_takes_into_account_encoding()
        {
            var input = ":>";
            var encodedInput = HttpUtility.HtmlEncode(input);
            var sut = new EmojiEnhancer();

            var context = new MessageProcessingContext(encodedInput);
            sut.Apply(context);

            context.CurrentMessage.Should().NotBe(encodedInput);
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