using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Common.ValueTypes
{
    public class ChunkedUploadType : IValidatableObject
    {
        public IBrowserFile Value { get; set; }

        public string FileName { get; set; }

        public string Path { get; set; }

        public long? Size { get; set; }

        public string SupportedFileTypes { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (validationContext.Items.ContainsKey(typeof(RequiredAttribute)) && Value == null)
            {
                results.Add(new ValidationResult("Required"));
            }

            return results;
        }
    }
}
