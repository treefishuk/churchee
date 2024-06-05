namespace Churchee.Common.Abstractions.Entities
{
    public interface ISourced
    {
        public string SourceName { get; }

        public string SourceId { get; }
    }
}
