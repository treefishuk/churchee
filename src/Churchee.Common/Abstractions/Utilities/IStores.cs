using Churchee.Common.Storage;

namespace Churchee.Common.Abstractions.Utilities
{
    public interface IStores
    {
        ISettingStore SettingStore { get; }

        IDataStore DataStore { get; }

        IBlobStore BlobStore { get; }

    }
}
