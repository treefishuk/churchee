using Xunit;

namespace Churchee.Test.Helpers.Validation
{
    public class ActionValidationTester
    {

        private readonly Action _instance;

        public ActionValidationTester(Action instance)
        {
            _instance = instance;
        }

        public void Throw<TId>(string exceptionMessage = "") where TId : Exception
        {
            var exception = Assert.Throws<TId>(_instance);

            if (!string.IsNullOrEmpty(exceptionMessage))
            {
                exception.Message.Should().Be(exceptionMessage);
            }
        }

        public void NotThrow()
        {
            Assert.NotNull(_instance);
        }

        public void NotBeNull()
        {
            Assert.NotNull(_instance);
        }
    }
}
