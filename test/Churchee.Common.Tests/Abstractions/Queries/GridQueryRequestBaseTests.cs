using Churchee.Common.Abstractions.Queries;
using Churchee.Test.Helpers.Validation;
using Moq;
using System.Reflection;

namespace Churchee.Common.Tests.Abstractions.Queries
{
    public class GridQueryRequestBaseTests
    {
        [Fact]
        public void GridQueryRequestBase_PassedOneAsSkip_SkipShouldReturnOne()
        {
            var mock = new Mock<GridQueryRequestBase<object>>(1, 1, string.Empty, string.Empty);

            mock.Object.Skip.Should().Be(1);
        }

        [Fact]
        public void GridQueryRequestBase_PassedTwoAsSkip_SkipShouldReturnTwo()
        {
            var mock = new Mock<GridQueryRequestBase<object>>(2, 1, string.Empty, string.Empty);

            mock.Object.Skip.Should().Be(2);
        }

        [Fact]
        public void GridQueryRequestBase_PassedOneAsTake_SkipShouldReturnOne()
        {
            var mock = new Mock<GridQueryRequestBase<object>>(1, 1, string.Empty, string.Empty);

            mock.Object.Take.Should().Be(1);
        }

        [Fact]
        public void GridQueryRequestBase_PassedTwoAsTake_SkipShouldReturnTwo()
        {
            var mock = new Mock<GridQueryRequestBase<object>>(1, 2, string.Empty, string.Empty);

            mock.Object.Take.Should().Be(2);
        }

        [Fact]
        public void GridQueryRequestBase_PassedTestAsSearchText_SearchTextShouldReturnTest()
        {
            var mock = new Mock<GridQueryRequestBase<object>>(1, 2, "Test", string.Empty);

            mock.Object.SearchText.Should().Be("Test");
        }

        [Fact]
        public void GridQueryRequestBase_PassedTestSpaceLowercaseDescAsOrderBy_OrderPropertiesValid()
        {
            var mock = new Mock<GridQueryRequestBase<object>>(1, 2, string.Empty, "Test desc");

            mock.Object.OrderBy.Should().Be("Test");
            mock.Object.OrderByDirection.Should().Be("desc");
        }

        [Fact]
        public void GridQueryRequestBase_PassedTestSpaceUpercaseDescAsOrderBy_OrderPropertiesValid()
        {
            var mock = new Mock<GridQueryRequestBase<object>>(1, 2, string.Empty, "Test Desc");

            mock.Object.OrderBy.Should().Be("Test");
            mock.Object.OrderByDirection.Should().Be("desc");
        }

        [Fact]
        public void GridQueryRequestBase_PassedTestAsOrderBy_OrderPropertiesValid()
        {
            var mock = new Mock<GridQueryRequestBase<object>>(1, 2, string.Empty, "Test");
            mock.Object.OrderBy.Should().Be("Test");
            mock.Object.OrderByDirection.Should().Be("asc");
        }

        [Fact]
        public void GridQueryRequestBase_PassedTestSpaceUnsupportedDirection_ReturnsArgumentOutOfRangeException()
        {
            // Act
            var exception = Assert.Throws<TargetInvocationException>(() =>
                new Mock<GridQueryRequestBase<object>>(1, 2, string.Empty, "Test Diagonally").Object);

            // Assert
            exception.InnerException?.Should().BeOfType<ArgumentOutOfRangeException>();
            exception.InnerException?.Message.Should().Be("Unsupported order direction (Parameter 'orderBy')");
        }

        [Fact]
        public void GridQueryRequestBase_NotPassedAnyOrdering_ShouldBeNull()
        {
            var mock = new Mock<GridQueryRequestBase<object>>(1, 2, string.Empty, string.Empty);
            mock.Object.OrderBy.Should().BeNull();
        }

    }
}
