using Xunit;

namespace Churchee.Test.Helpers.Validation
{
    public class ListValidationTester<T>
    {
        private readonly IList<T> _instance;

        public ListValidationTester(IList<T> instance)
        {
            _instance = instance;
        }

        public void BeEmpty()
        {
            Assert.Empty(_instance);
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
            Assert.Equal(_instance.Count, count);
        }

        public void Contain(T v)
        {
            Assert.Contains(_instance, x => x != null && x.Equals(v));
        }
    }
}
