using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Common.ValueTypes
{
    public class ChunkedImageUploadType : IValidatableObject
    {
        public ChunkedImageUploadType()
        {
            TempFilePath = string.Empty;
            Path = string.Empty;
            ThumbnailUrl = string.Empty;
            SupportedFileTypes = string.Empty;
        }

        public IBrowserFile File { get; set; }

        public string TempFilePath { get; set; }

        public string ThumbnailUrl { get; set; }

        public string Path { get; set; }

        public string SupportedFileTypes { get; set; }

        [Required]
        public string Description { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (validationContext.Items.ContainsKey(typeof(RequiredAttribute)) && File == null)
            {
                results.Add(new ValidationResult("Required"));
            }

            return results;
        }
    }
}
