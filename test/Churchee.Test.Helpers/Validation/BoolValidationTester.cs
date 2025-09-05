using Xunit;

namespace Churchee.Test.Helpers.Validation
{
    public class BoolValidationTester : ValidationTester<bool>
    {
        private readonly bool _instance;

        public BoolValidationTester(bool instance) : base(instance)
        {
            _instance = instance;
        }

        public void BeTrue()
        {
            Assert.True(_instance);
        }

        public void BeFalse()
        {
            Assert.False(_instance);
        }
    }
}
