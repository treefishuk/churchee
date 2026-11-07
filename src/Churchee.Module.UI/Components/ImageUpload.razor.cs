using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;
using Churchee.Common.ValueTypes;
using Churchee.ImageProcessing.Jobs;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Churchee.Module.UI.Components
{
    public partial class ImageUpload : IDisposable
    {
        [Inject]
        protected ITenantResolver TenantResolver { get; set; } = default!;

        [Inject]
        protected IImageProcessor ImageProcessor { get; set; } = default!;

        [Inject]
        protected IBlobStore BlobStore { get; set; } = default!;

        [Inject]
        public IJobService JobService { get; set; } = default!;

        [Parameter]
        public ImageUploadType Model { get; set; } = default!;

        [Parameter]
        public EventCallback<ImageUploadType> ModelChanged { get; set; }

        [Parameter]
        public int MaxUploadSize { get; set; } = 5;

        private double uploadProgress = 0;

        private string status = string.Empty;

        private bool generating = false;

        private CancellationTokenSource? _descriptionCts;

        private bool disposedValue;

        public static async Task<string> ToBase64ImageAsync(Stream imageStream, string contentType)
        {
            using var ms = new MemoryStream();
            await imageStream.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();

            return $"data:{contentType};base64,{Convert.ToBase64String(bytes)}";
        }

        private async Task OnDescriptionChange()
        {
            await ModelChanged.InvokeAsync(Model);
            if (_descriptionCts != null)
            {
                await _descriptionCts.CancelAsync();
            }
            generating = false;
            StateHasChanged();
        }

        async Task OnImageUploadInputChangeAsync(ImageUploadType upload, UploadChangeEventArgs args)
        {
            if (upload.ApplicationTenantId == Guid.Empty)
            {
                upload.ApplicationTenantId = TenantResolver.GetTenantId();
            }

            // Cancel any previous alt text generation
            _ = (_descriptionCts?.CancelAsync());
            _descriptionCts = new CancellationTokenSource();

            var file = args.Files?.FirstOrDefault();

            if (file == null)
            {
                return;
            }

            upload.File = file;
            upload.FileName = file.Name;
            upload.Size = file.Size;
            upload.ContentType = file.ContentType;
            upload.Value = string.Empty;
            uploadProgress = 0;

            string fileName = Path.GetFileNameWithoutExtension(upload.FileName);

            await using var inputStream = file.OpenReadStream(long.MaxValue);

            await using var originalImageStream = new MemoryStream();

            UpdateProgress(20);

            await inputStream.CopyToAsync(originalImageStream, CancellationToken.None);

            originalImageStream.Position = 0;

            UpdateProgress(20);

            await using var webpStream = await ImageProcessor.ConvertToWebP(originalImageStream, CancellationToken.None);

            UpdateProgress(30);

            string imagePath = $"{upload.Path}/{fileName.ToDevName()}.webp";

            upload.FileName = await BlobStore.SaveAsync(upload.ApplicationTenantId, imagePath, webpStream, false, CancellationToken.None);

            UpdateProgress(80);

            JobService.QueueJob<ImageCropsGenerator>(x => x.CreateCropsAsync(upload.ApplicationTenantId, upload.FileName, true, CancellationToken.None));

            originalImageStream.Position = 0;

            upload.ThumbnailUrl = await GenerateThumbnailPreviewAsync(originalImageStream, 200, CancellationToken.None);

            UpdateProgress(100);

        }

        private void UpdateProgress(int value)
        {
            uploadProgress = value;

            StateHasChanged();
        }


        private async Task<string> GenerateThumbnailPreviewAsync(Stream imageStream, int maxDimension, CancellationToken cancellationToken)
        {
            if (imageStream == null)
            {
                return string.Empty;
            }

            if (imageStream.CanSeek && imageStream.Length == 0)
            {
                return string.Empty;
            }

            try
            {
                if (imageStream.CanSeek)
                {
                    imageStream.Position = 0;
                }

                await using var resized = await ImageProcessor.ResizeImageAsync(imageStream, maxDimension, 0, ".webp", cancellationToken);

                await using var output = new MemoryStream();
                await resized.CopyToAsync(output, 64 * 1024, cancellationToken);
                var bytes = output.ToArray();

                return $"data:image/webp;base64,{Convert.ToBase64String(bytes)}";
            }
            catch (OperationCanceledException)
            {
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                _descriptionCts?.Dispose();
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}