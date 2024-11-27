using Azure.Storage.Blobs;
using Churchee.Common.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Churchee.Blobstorage.Providers.Azure
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

            var exists = await client.ExistsAsync(cancellationToken);

            var blob = client.GetBlobClient(fullPath);

            return await blob.OpenReadAsync(cancellationToken: cancellationToken);
        }

        public async Task<string> SaveAsync(Guid applicationTenantId, string fullPath, Stream stream, bool overrideExisting = false, CancellationToken cancellationToken = default)
        {
            // With container URL and DefaultAzureCredential
            var client = new BlobContainerClient(_connectionString, applicationTenantId.ToString());

            var exists = await client.ExistsAsync(cancellationToken);

            if (!exists)
            {
                await client.CreateAsync(cancellationToken: cancellationToken);
            }

            if (overrideExisting)
            {
                await client.DeleteBlobIfExistsAsync(fullPath, cancellationToken: cancellationToken);
            }


            if (!overrideExisting)
            {

                var existingBlob = client.GetBlobClient(fullPath);

                bool alreadyExists = await existingBlob.ExistsAsync(cancellationToken: cancellationToken);


                if (alreadyExists)
                {

                    int i = 0;

                    string extension = Path.GetExtension(fullPath);

                    string fileName = Path.GetFileNameWithoutExtension(fullPath);

                    string folderpath = fullPath.Replace(extension, "").Replace(fileName, "");

                    bool stillExists = true;

                    while (stillExists)
                    {
                        fullPath = $"{folderpath}{fileName}({i}){extension}";

                        var blob = client.GetBlobClient(fullPath);

                        stillExists = await blob.ExistsAsync(cancellationToken: cancellationToken);

                        i++;
                    }
                }
            }

            await client.UploadBlobAsync(fullPath, stream, cancellationToken);

            return fullPath;
        }



    }
}
