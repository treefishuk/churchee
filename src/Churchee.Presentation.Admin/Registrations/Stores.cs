using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;

namespace Churchee.Presentation.Admin.Registrations
{
    public class Stores : IStores
    {
        public Stores(ISettingStore settingStore, IDataStore dataStore, IBlobStore blobStore)
        {
            SettingStore = settingStore;
            DataStore = dataStore;
            BlobStore = blobStore;
        }

        public ISettingStore SettingStore { get; }

        public IDataStore DataStore { get; }

        public IBlobStore BlobStore { get; }

    }
}
