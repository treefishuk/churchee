using Azure.Storage.Blobs;
using Churchee.Common.Abstractions.Utilities;
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
        private readonly IImageProcessor _imageProcessor;

        public AzureBlobStore(IConfiguration configuartion, IImageProcessor imageProcessor)
        {
            _connectionString = configuartion.GetConnectionString("Storage");
            _imageProcessor = imageProcessor;
        }

        public async Task<Stream> GetAsync(Guid applicationTenantId, string fullPath, CancellationToken cancellationToken = default)
        {
            // With container URL and DefaultAzureCredential
            var client = new BlobContainerClient(_connectionString, applicationTenantId.ToString());

            var blob = client.GetBlobClient(fullPath);

            return await blob.OpenReadAsync();
        }

        public async Task<string> SaveAsync(Guid applicationTenantId, string fullPath, Stream stream, bool overrideExisting = false, bool createCrops = false, CancellationToken cancellationToken = default)
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


            if (createCrops)
            {
                string extension = Path.GetExtension(fullPath);

                string fileName = Path.GetFileNameWithoutExtension(fullPath);

                string folderpath = fullPath.Replace(extension, "").Replace(fileName, "");

                await CreateCrop(fileName, folderpath, extension, "t", stream, client, 200, cancellationToken);
                await CreateCrop(fileName, folderpath, extension, "s", stream, client, 400, cancellationToken);
                await CreateCrop(fileName, folderpath, extension, "m", stream, client, 800, cancellationToken);
                await CreateCrop(fileName, folderpath, extension, "l", stream, client, 2000, cancellationToken);

            }



            return fullPath;
        }

        private async Task CreateCrop(string fileName, string folderPath, string extension, string suffix, Stream stream, BlobContainerClient client, int width, CancellationToken cancellationToken)
        {
            string cropPath = $"{folderPath}{fileName}_{suffix}{extension}";

            stream.Position = 0;

            var smallImageStream = _imageProcessor.ResizeImage(stream, width, 0, extension);

            await client.UploadBlobAsync(cropPath, smallImageStream, cancellationToken);
        }
    }
}
