using Churchee.Common.Storage;

namespace Churchee.Common.Abstractions.Storage
{
    public interface ISeedData
    {
        void SeedData(IDataStore storage);

        int Order { get; }
    }
}
