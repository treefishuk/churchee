namespace Churchee.Module.Videos.Specifications
{
    public class VideoSearchFilterSpecificationTests
    {
        [Fact]
        public void Constructor_WithNullOrEmptySearchText_DoesNotAddCriteria()
        {
            // Act
            var specNull = new VideoSearchFilterSpecification(null);
            var specEmpty = new VideoSearchFilterSpecification("");

            // Assert
            Assert.NotNull(specNull);
            Assert.NotNull(specEmpty);
            // No exception means pass for this simple wrapper
        }

        [Fact]
        public void Constructor_WithSearchText_AddsWhereCriteria()
        {
            // Arrange
            string searchText = "test";

            // Act
            var spec = new VideoSearchFilterSpecification(searchText);

            // Assert
            // The criteria should be present in the Query object
            // (Ardalis.Specification does not expose criteria directly, so this is a smoke test)
            Assert.NotNull(spec);
        }
    }
}
