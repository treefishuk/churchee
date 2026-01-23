using System.Linq.Dynamic.Core;

namespace Churchee.Common.Tests.Extensions
{
    public class IQueryableExtensionsTests
    {
        private class TestItem
        {
            public TestItem()
            {
                Name = string.Empty;
            }

            public int Id { get; set; }
            public string Name { get; set; }
        }

        [Fact]
        public void OrderBy_ValidPropertyAscending_ReturnsOrdered()
        {
            var data = new List<TestItem>
            {
                new() { Id = 2, Name = "B" },
                new() { Id = 1, Name = "A" }
            }.AsQueryable();

            var result = data.OrderBy("Id", "asc").ToList();

            Assert.Equal(1, result[0].Id);
            Assert.Equal(2, result[1].Id);
        }

        [Fact]
        public void OrderBy_ValidPropertyDescending_ReturnsOrdered()
        {
            var data = new List<TestItem>
            {
                new() { Id = 1, Name = "A" },
                new() { Id = 2, Name = "B" }
            }.AsQueryable();

            var result = data.OrderBy("Id", "desc").ToList();

            Assert.Equal(2, result[0].Id);
            Assert.Equal(1, result[1].Id);
        }

        [Fact]
        public void OrderBy_NullOrEmptyPropertyName_ReturnsUnchanged()
        {
            var data = new List<TestItem>
            {
                new() { Id = 2, Name = "B" },
                new() { Id = 1, Name = "A" }
            }.AsQueryable();

            var resultNull = data.OrderBy(null, "asc").ToList();
            var resultEmpty = data.OrderBy("", "asc").ToList();

            Assert.Equal(data.ToList(), resultNull);
            Assert.Equal(data.ToList(), resultEmpty);
        }

        [Fact]
        public void OrderBy_InvalidProperty_ThrowsException()
        {
            var data = new List<TestItem>
            {
                new() { Id = 1, Name = "A" }
            }.AsQueryable();

            Assert.Throws<InvalidOperationException>(() => data.OrderBy("NonExistent", "asc").ToList());
        }
    }
}
