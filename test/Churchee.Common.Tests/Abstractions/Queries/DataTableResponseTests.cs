using Churchee.Common.Abstractions;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Common.Tests.Abstractions.Queries
{
    public class DataTableResponseTests
    {
        [Fact]
        public void Properties_Should_Be_Set_And_Retrieved_Correctly()
        {
            // Arrange
            var response = new DataTableResponse<string>();

            // Act
            response.Draw = 1;
            response.RecordsTotal = 100;
            response.RecordsFiltered = 50;
            response.Data = new List<string> { "Item1", "Item2" };
            response.Error = "No error";

            // Assert
            response.Draw.Should().Be(1);
            response.RecordsTotal.Should().Be(100);
            response.RecordsFiltered.Should().Be(50);
            response.Data.Should().HaveCount(2);
            response.Error.Should().Be("No error");
        }

        [Fact]
        public void Default_Values_Should_Be_Correct()
        {
            // Arrange & Act
            var response = new DataTableResponse<string>();

            // Assert
            response.Draw.Should().Be(0);
            response.RecordsTotal.Should().Be(0);
            response.RecordsFiltered.Should().Be(0);
            response.Data.Should().BeNull();
            response.Error.Should().BeNull();
        }
    }
}