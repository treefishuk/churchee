namespace Churchee.Common.ResponseTypes
{
    public class ValidationError
    {
        public ValidationError(string error, string code)
        {
            Description = error;
            Property = code;
        }

        public string Description { get; private set; }
        public string Property { get; private set; }
    }
}
