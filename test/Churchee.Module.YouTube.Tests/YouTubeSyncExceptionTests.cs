using Churchee.Module.YouTube.Exceptions;

namespace Churchee.Module.YouTube.Tests
{
    public class YouTubeSyncExceptionTests
    {
        [Fact]
        public void Default_ctor_creates_exception()
        {
            var ex = new YouTubeSyncException();

            Assert.NotNull(ex);
        }

        [Fact]
        public void Message_ctor_sets_message()
        {
            var ex = new YouTubeSyncException("boom");

            Assert.Equal("boom", ex.Message);
        }

        [Fact]
        public void InnerException_ctor_sets_inner()
        {
            var inner = new InvalidOperationException("inner");
            var ex = new YouTubeSyncException("outer", inner);

            Assert.Equal("outer", ex.Message);
            Assert.Same(inner, ex.InnerException);
        }
    }
}
