using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Common.ValueTypes
{
    public class Upload : IValidatableObject
    {
        public Upload()
        {

        }
        public Upload(string path)
        {
            Path = path;
        }

        public string Value { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
        public long? Size { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (validationContext.ImplementsInterface<RequiredAttribute>())
            {
                if (string.IsNullOrEmpty(Value))
                {
                    results.Add(new ValidationResult("Required"));
                }
            }

            return results;
        }
    }
}
