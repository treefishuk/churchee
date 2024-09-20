namespace Churchee.Common.ResponseTypes
{
    public class ValidationError
    {
        public ValidationError(string error, string code)
        {
            Description = error;
            Property = code;
        }

        public string Description { get; }

        public string Property { get; }
    }
}
