using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Common.ValueTypes
{
    public class Upload : IValidatableObject
    {
        public string Value { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public long? Size { get; set; }
        public string SupportedFileTypes { get; set; } = string.Empty;
        public byte[] FileBytes { get; set; } = [];
        public string ContentType { get; set; } = string.Empty;
        public string PreviewUrl { get; set; } = string.Empty;
        public IBrowserFile File { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (validationContext.Items.ContainsKey(typeof(RequiredAttribute)) && File == null && FileBytes.Length == 0 && string.IsNullOrWhiteSpace(Value))
            {
                results.Add(new ValidationResult("Required"));
            }

            return results;
        }
    }
}
