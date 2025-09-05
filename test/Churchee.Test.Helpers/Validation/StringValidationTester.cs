using Xunit;

namespace Churchee.Test.Helpers.Validation
{
    public class StringValidationTester : ValidationTester<string>
    {
        private readonly string _instance;

        public StringValidationTester(string instance) : base(instance)
        {
            _instance = instance;
        }

        public void BeEmpty()
        {
            Assert.Equal(string.Empty, _instance);
        }
    }
}
