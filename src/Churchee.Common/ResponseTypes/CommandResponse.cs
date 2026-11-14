namespace Churchee.Common.ResponseTypes
{
    public class CommandResponse
    {
        public CommandResponse()
        {
            Errors = [];
        }

        public void AddError(string error, string propertyName)
        {
            Errors.Add(new ValidationError(error, propertyName));
        }

        public ICollection<ValidationError> Errors { get; }

        public bool IsSuccess => Errors.Count == 0;
    }
}
