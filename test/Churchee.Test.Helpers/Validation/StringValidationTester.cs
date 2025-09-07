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

        public void NotBeEmpty()
        {
            Assert.NotEqual(string.Empty, _instance);
        }

        public void ContainAll(params string[] strings)
        {
            foreach (string str in strings)
            {
                Assert.Contains(str, _instance);
            }
        }

        public void Contain(string str)
        {
            Assert.Contains(str, _instance);
        }
    }
}
