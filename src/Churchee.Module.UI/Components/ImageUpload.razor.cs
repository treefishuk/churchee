using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ValueTypes;
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
        private IAiToolUtilities AiToolUtilities { get; set; } = default!;

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

            using var stream = file.OpenReadStream(file.Size);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            upload.FileBytes = ms.ToArray();

            upload.ThumbnailUrl = await GenerateThumbnailPreviewAsync(upload.FileBytes, 200);
        }

        private async Task<string> GenerateThumbnailPreviewAsync(byte[] imageBytes, int maxDimension)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                return string.Empty;
            }

            try
            {
                UpdateProgress(20);

                await using var input = new MemoryStream(imageBytes);
                await using var resized = await ImageProcessor.ResizeImageAsync(input, maxDimension, 0, ".webp", CancellationToken.None);

                UpdateProgress(70);

                resized.Position = 0;

                using var output = new MemoryStream();
                await resized.CopyToAsync(output);

                UpdateProgress(90);

                string result = Convert.ToBase64String(output.ToArray());

                UpdateProgress(100);

                return $"data:image/webp;base64,{result}";
            }
            catch
            {
                return string.Empty;
            }
        }

        private void UpdateProgress(int value)
        {
            uploadProgress = value;

            StateHasChanged();
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