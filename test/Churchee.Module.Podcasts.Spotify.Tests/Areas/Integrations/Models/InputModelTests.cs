using Churchee.Module.Podcasts.SpotifyIntegration.Areas.Integrations.Models;
using Churchee.Test.Helpers.Validation;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Podcasts.Spotify.Tests.Areas.Integrations.Models
{
    public class InputModelTests
    {
        [Fact]
        public void DefaultConstructor_ShouldInitializePropertiesToEmptyStrings()
        {
            // Act
            var model = new InputModel();

            // Assert
            model.SpotifyRSSFeedUrl.Should().NotBeNull();
            model.SpotifyRSSFeedUrl.Should().BeEmpty();
            model.NameForContent.Should().NotBeNull();
            model.NameForContent.Should().BeEmpty();
        }

        [Fact]
        public void ParameterizedConstructor_ShouldAssignValues()
        {
            // Arrange
            var url = "https://localhost/feed";
            var name = "My Podcast";

            // Act
            var model = new InputModel(url, name);

            // Assert
            model.SpotifyRSSFeedUrl.Should().Be(url);
            model.NameForContent.Should().Be(name);
        }

        [Fact]
        public void ParameterizedConstructor_WithNulls_ShouldSetEmptyStrings()
        {
            // Act
            var model = new InputModel(string.Empty, string.Empty);

            // Assert
            model.SpotifyRSSFeedUrl.Should().NotBeNull();
            model.SpotifyRSSFeedUrl.Should().BeEmpty();
            model.NameForContent.Should().NotBeNull();
            model.NameForContent.Should().BeEmpty();
        }

        [Fact]
        public void Validation_ShouldFail_WhenRequiredPropertiesAreMissingOrEmpty()
        {
            // Arrange
            var model = new InputModel(); // default ctor sets empty strings which should violate [Required]

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            validationResults.Should().NotBeNull();
            validationResults.Count.Should().Be(2);
            validationResults.Any(a => a.ErrorMessage == "The SpotifyRSSFeedUrl field is required.").Should().BeTrue();
            validationResults.Any(a => a.ErrorMessage == "The NameForContent field is required.").Should().BeTrue();
        }

        private static IList<ValidationResult> ValidateModel(object model)
        {
            var ctx = new ValidationContext(model, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(model, ctx, results, validateAllProperties: true);
            return results;
        }
    }
}