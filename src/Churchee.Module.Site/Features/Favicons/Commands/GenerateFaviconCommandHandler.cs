using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using MediatR;


namespace Churchee.Module.Site.Features.Favicons.Commands
{
    public class GenerateFaviconCommandHandler : IRequestHandler<GenerateFaviconCommand, CommandResponse>
    {

        private readonly IBlobStore _blobStore;
        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;
        private readonly IImageProcessor _imageProcessor;

        public GenerateFaviconCommandHandler(IBlobStore blobStore, IDataStore dataStore, ICurrentUser currentUser, IImageProcessor imageProcessor)
        {
            _blobStore = blobStore;
            _dataStore = dataStore;
            _currentUser = currentUser;
            _imageProcessor = imageProcessor;
        }

        public async Task<CommandResponse> Handle(GenerateFaviconCommand request, CancellationToken cancellationToken)
        {
            byte[] data = Convert.FromBase64String(request.Base64Content.Split(',')[1]);

            using var originalImageStream = new MemoryStream(data);

            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            await CreatePngFavicon(originalImageStream, applicationTenantId, "android-chrome-", 512, cancellationToken);
            await CreatePngFavicon(originalImageStream, applicationTenantId, "android-chrome-", 192, cancellationToken);
            await CreatePngFavicon(originalImageStream, applicationTenantId, "favicon-", 48, cancellationToken);
            await CreatePngFavicon(originalImageStream, applicationTenantId, "favicon-", 32, cancellationToken);
            await CreatePngFavicon(originalImageStream, applicationTenantId, "favicon-", 16, cancellationToken);
            await CreateAppleTouchIcon(originalImageStream, applicationTenantId, cancellationToken);
            await CreateFavicon(originalImageStream, applicationTenantId, cancellationToken);
            await CreateAdminThumbnail(originalImageStream, applicationTenantId, cancellationToken);

            var media = new MediaItem(applicationTenantId, "Favicon", "/favicons/apple-touch-icon.png", string.Empty, string.Empty, request.FolderId, string.Empty, string.Empty);

            _dataStore.GetRepository<MediaItem>().Create(media);

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }

        private async Task CreatePngFavicon(MemoryStream originalImageStream, Guid applicationTenantId, string fileNamePrefix, int size, CancellationToken cancellationToken)
        {
            string imagePath = $"/favicons/{fileNamePrefix}{size}x{size}.png";

            using var imageStream = await _imageProcessor.ResizeImageAsync(originalImageStream, size, 0, ".png", cancellationToken);

            await _blobStore.SaveAsync(applicationTenantId, imagePath, imageStream, true, cancellationToken);

            originalImageStream.Position = 0; // Reset stream position for next use
        }

        private async Task CreateAppleTouchIcon(MemoryStream originalImageStream, Guid applicationTenantId, CancellationToken cancellationToken)
        {
            string imagePath = $"/favicons/apple-touch-icon.png";

            using var imageStream = await _imageProcessor.ResizeImageAsync(originalImageStream, 256, 0, ".png", cancellationToken);

            await _blobStore.SaveAsync(applicationTenantId, imagePath, imageStream, true, cancellationToken);

            originalImageStream.Position = 0; // Reset stream position for next use
        }

        private async Task CreateAdminThumbnail(MemoryStream originalImageStream, Guid applicationTenantId, CancellationToken cancellationToken)
        {
            string imagePath = $"/favicons/apple-touch-icon_s.png";

            using var imageStream = await _imageProcessor.ResizeImageAsync(originalImageStream, 575, 0, ".png", cancellationToken);

            await _blobStore.SaveAsync(applicationTenantId, imagePath, imageStream, true, cancellationToken);

            originalImageStream.Position = 0; // Reset stream position for next use
        }

        private async Task CreateFavicon(MemoryStream originalImageStream, Guid applicationTenantId, CancellationToken cancellationToken)
        {
            int[] sizes = { 16, 32, 48, 256 };
            var imageDataList = new List<byte[]>();

            // Reset stream position before reading
            originalImageStream.Position = 0;


            foreach (int size in sizes)
            {
                using var imageStream = await _imageProcessor.ResizeImageAsync(originalImageStream, size, 0, ".png", cancellationToken);
                imageStream.Position = 0;
                using var ms = new MemoryStream();
                imageStream.CopyTo(ms);
                imageDataList.Add(ms.ToArray());
                originalImageStream.Position = 0;
            }

            using var outStream = new MemoryStream();
            using (var bw = new BinaryWriter(outStream, System.Text.Encoding.UTF8, true))
            {
                // ICONDIR header
                bw.Write((short)0); // Reserved
                bw.Write((short)1); // Type (1 = icon)
                bw.Write((short)sizes.Length); // Number of images

                int dataOffset = 6 + (16 * sizes.Length);

                for (int i = 0; i < sizes.Length; i++)
                {
                    int size = sizes[i];
                    byte[] imageData = imageDataList[i];

                    bw.Write((byte)(size == 256 ? 0 : size)); // Width
                    bw.Write((byte)(size == 256 ? 0 : size)); // Height
                    bw.Write((byte)0); // Color palette
                    bw.Write((byte)0); // Reserved
                    bw.Write((short)1); // Color planes
                    bw.Write((short)32); // Bits per pixel
                    bw.Write(imageData.Length); // Image data size
                    bw.Write(dataOffset); // Offset
                    dataOffset += imageData.Length;
                }

                // Write image data
                foreach (byte[] imageData in imageDataList)
                {
                    bw.Write(imageData);
                }
            }

            outStream.Position = 0;
            string icoPath = "/favicons/favicon.ico";
            await _blobStore.SaveAsync(applicationTenantId, icoPath, outStream, true, cancellationToken);
        }
    }
}
