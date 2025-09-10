using Churchee.Module.Site.Areas.Site.Models;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Tests.Areas.Site.Models
{
    public class CreateFaviconModelTests
    {
        [Fact]
        public void File_IsRequired()
        {
            var model = new CreateFaviconModel
            {
                File = null
            };

            var results = ValidateModel(model);

            Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateFaviconModel.File)) && (r.ErrorMessage != null) && r.ErrorMessage.Contains("required", StringComparison.OrdinalIgnoreCase));
        }

        private static List<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }
    }
}