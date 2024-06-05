using System;
using System.Collections.Generic;
using System.Linq;

namespace Churchee.Common.ResponseTypes
{
    public class CommandResponse
    {
        public CommandResponse()
        {
            Errors = new List<ValidationError>();
        }

        public void AddError(string error, string propertyName)
        {
            Errors.Add(new ValidationError(error, propertyName));
        }

        public List<ValidationError> Errors { get; }

        public bool IsSuccess => Errors.Count() == 0;
    }
}
