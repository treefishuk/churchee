using Xunit;

namespace Churchee.Test.Helpers.Validation
{
    public class ValidationTester<T>
    {
        private readonly T _instance;

        public ValidationTester(T instance)
        {
            _instance = instance;
        }

        public void BeNull()
        {
            Assert.Null(_instance);
        }

        public void NotBeNull()
        {
            Assert.NotNull(_instance);
        }

        public void Be(T match)
        {
            Assert.Equal(match, _instance);
        }

        public void NotBe(T match)
        {
            Assert.NotEqual(match, _instance);
        }

        public void BeOfType<TType>()
        {
            Assert.IsType<TType>(_instance, true);
        }

        public void BeAssignableTo<TType>()
        {
            Assert.IsType<TType>(_instance, exactMatch: false);
        }

        public void BeEquivalentTo(T equivalent)
        {
            Assert.Equivalent(equivalent, _instance);
        }


    }
}
