using Xunit;

namespace Churchee.Test.Helpers.Validation
{
    public class AsyncMethodValidationTester
    {

        private readonly Func<Task> _instance;

        public AsyncMethodValidationTester(Func<Task> instance)
        {
            _instance = instance;
        }

        public async Task ThrowAsync<TId>(string exceptionMessage = "") where TId : Exception
        {
            var exception = await Assert.ThrowsAsync<TId>(_instance);

            if (!string.IsNullOrEmpty(exceptionMessage))
            {
                exception.Message.Should().Be(exceptionMessage);
            }
        }

        public void NotThrow()
        {
            Assert.NotNull(_instance);
        }
    }
}
