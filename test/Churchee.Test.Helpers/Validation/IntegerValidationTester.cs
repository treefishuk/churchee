using Xunit;

namespace Churchee.Test.Helpers.Validation
{
    public class IntegerValidationTester : ValidationTester<int>
    {
        private readonly int _instance;

        public IntegerValidationTester(int instance) : base(instance)
        {
            _instance = instance;
        }

        public void BeGreaterThan(int match)
        {
            Assert.True(_instance > match);
        }

        public void BeLessThan(int match)
        {
            Assert.True(_instance < match);
        }
    }
}
