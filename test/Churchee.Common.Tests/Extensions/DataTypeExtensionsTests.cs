using Churchee.Test.Helpers.Validation;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Common.Tests.Extensions
{
    public class DataTypeExtensionsTests
    {
        [Fact]
        public void DataTypes_Values_MatchExpected()
        {
            DataTypes.Hidden.Should().Be("Hidden");
            DataTypes.DateTime.Should().Be("DateTime");
            DataTypes.Date.Should().Be("Date");
            DataTypes.Time.Should().Be("Time");
            DataTypes.PhoneNumber.Should().Be("PhoneNumber");
            DataTypes.Currency.Should().Be("Currency");
            DataTypes.Text.Should().Be("Text");
            DataTypes.Html.Should().Be("Html");
            DataTypes.MultilineText.Should().Be("MultilineText");
            DataTypes.EmailAddress.Should().Be("EmailAddress");
            DataTypes.Password.Should().Be("Password");
            DataTypes.Url.Should().Be("Url");
            DataTypes.ImageUpload.Should().Be("ImageUpload");
            DataTypes.ImageUrl.Should().Be("ImageUrl");
            DataTypes.GeoCoordinates.Should().Be("GeoCoordinates");
            DataTypes.PostalCode.Should().Be("PostalCode");
            DataTypes.Upload.Should().Be("Upload");
            DataTypes.CssEditor.Should().Be("CssEditor");
            DataTypes.RazorEditor.Should().Be("RazorEditor");
            DataTypes.Readonly.Should().Be("Readonly");
            DataTypes.TextWithSlug.Should().Be("TextWithSlug");
            DataTypes.CheckboxList.Should().Be("CheckboxList");

        }



    }
}
