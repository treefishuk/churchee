using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Churchee.Common.Storage;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Churchee.Blobstorage.Providers.Azure
{
    public class AzureBlobStore : IBlobStore
    {
        private readonly string _connectionString;

        public AzureBlobStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Storage");
        }

        public async Task<Stream> GetReadStreamAsync(Guid applicationTenantId, string fullPath, CancellationToken cancellationToken = default)
        {
            // With container URL and DefaultAzureCredential
            var client = new BlobContainerClient(_connectionString, applicationTenantId.ToString());

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

                    int i = 1;

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

        public async Task WriteChunksAsync(Guid applicationTenantId, string fullPath, IBrowserFile file, IProgress<double> progress, CancellationToken cancellationToken = default)
        {
            var client = new BlobContainerClient(_connectionString, applicationTenantId.ToString());

            var exists = await client.ExistsAsync(cancellationToken);

            if (!exists)
            {
                await client.CreateAsync(cancellationToken: cancellationToken);
            }

            var blobClient = client.GetBlockBlobClient(fullPath);

            long maxFileSize = 100 * 1024 * 1024; // 100MB

            using var fileStream = file.OpenReadStream(maxAllowedSize: maxFileSize, cancellationToken: cancellationToken);

            int blockSize = 1 * 1024 * 1024; // 1MB
            var blockIds = new List<string>();
            int index = 0;
            long totalBytes = fileStream.Length;
            long bytesUploaded = 0;


            while (fileStream.Position < fileStream.Length)
            {
                byte[] buffer = new byte[blockSize];
                int bytesRead = await fileStream.ReadAsync(buffer.AsMemory(0, blockSize), cancellationToken);
                string blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(index.ToString("d6")));
                using var ms = new MemoryStream(buffer, 0, bytesRead);
                await blobClient.StageBlockAsync(blockId, ms, cancellationToken: cancellationToken);
                blockIds.Add(blockId);
                bytesUploaded += bytesRead;

                // Report progress
                progress?.Report((double)bytesUploaded / totalBytes * 100);

                index++;
            }

            await blobClient.CommitBlockListAsync(blockIds, cancellationToken: cancellationToken);
        }
    }
}
