using Azure.Storage.Blobs;
using Churchee.Common.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Churchee.Blobstorage.Providers
{
    public class AzureBlobStore : IBlobStore
    {
        private readonly string _connectionString;

        public AzureBlobStore(IConfiguration configuartion)
        {
            _connectionString = configuartion.GetConnectionString("Storage");
        }

        public async Task<Stream> GetAsync(Guid applicationTenantId, string fullPath, CancellationToken cancellationToken = default)
        {
            // With container URL and DefaultAzureCredential
            var client = new BlobContainerClient(_connectionString, applicationTenantId.ToString());

            var blob = client.GetBlobClient(fullPath);

            return await blob.OpenReadAsync();
        }

        public async Task SaveAsync(Guid applicationTenantId, string fullPath, Stream stream, bool overrideExisting = false, CancellationToken cancellationToken = default)
        {
            // With container URL and DefaultAzureCredential
            var client = new BlobContainerClient(_connectionString, applicationTenantId.ToString());

            var exists = client.Exists();

            if (!exists)
            {
                client.Create();
            }

            await client.DeleteBlobIfExistsAsync(fullPath, cancellationToken: cancellationToken);

            await client.UploadBlobAsync(fullPath, stream, cancellationToken);
        }
    }
}
