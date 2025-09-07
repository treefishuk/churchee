using Xunit;

namespace Churchee.Test.Helpers.Validation
{
    public class EnumerableValidationTester<T>
    {
        private readonly IEnumerable<T> _instance;

        public EnumerableValidationTester(IEnumerable<T> instance)
        {
            _instance = instance;
        }

        public void BeEmpty()
        {
            Assert.Empty(_instance);
        }

        public void BeNull()
        {
            Assert.Null(_instance);
        }


        public void ContainSingle(Predicate<T> predicate)
        {
            Assert.Single(_instance, predicate);
        }

        public void ContainSingle()
        {
            Assert.Single(_instance);
        }

        public void NotBeNull()
        {
            Assert.NotNull(_instance);
        }

        public void HaveCount(int count)
        {
            Assert.Equal(_instance.Count(), count);
        }
    }
}
