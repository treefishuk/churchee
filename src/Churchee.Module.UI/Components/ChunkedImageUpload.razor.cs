using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ValueTypes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;

namespace Churchee.Module.UI.Components
{
    public partial class ChunkedImageUpload : IDisposable
    {
        [Inject]
        protected ITenantResolver TenantResolver { get; set; } = default!;

        [Inject]
        protected IImageProcessor ImageProcessor { get; set; } = default!;

        [Parameter]
        public ChunkedImageUploadType Model { get; set; } = default!;

        [Parameter]
        public EventCallback<ChunkedImageUploadType> ModelChanged { get; set; }

        [Parameter]
        public int MaxUploadSize { get; set; } = 5;

        private double uploadProgress = 0;

        private string status = string.Empty;

        private string tempSmallImage = string.Empty;

        private CancellationTokenSource? _descriptionCts;

        private bool disposedValue;


        private async Task OnDescriptionChanged(string? value)
        {
            Model.Description = value;

            // Debounce so we don't call ModelChanged on every keystroke
            _descriptionCts?.Cancel();
            _descriptionCts = new CancellationTokenSource();
            var token = _descriptionCts.Token;

            try
            {
                await Task.Delay(400, token); // adjust debounce interval as needed
            }
            catch (OperationCanceledException)
            {
                return;
            }

            if (Model.File != null)
            {
                Model.TempFilePath = string.Empty;
            }

            await ModelChanged.InvokeAsync(Model);
        }



        private async Task HandleFileUpload(UploadChangeEventArgs e)
        {
            if (e.Files?.FirstOrDefault() is not IBrowserFile file)
            {
                uploadProgress = 0;

                status = string.Empty;

                StateHasChanged();

                return;
            }

            Model.File = file;

            Model.TempFilePath = await UploadTempFileInChunksAsync(file, CancellationToken.None);

            status = $"Uploaded {file.Name}";

            StateHasChanged();

            try
            {
                // Get a read stream for the uploaded file and write temp file
                await using var tempFileStream = File.Create(Model.TempFilePath);
                await file.OpenReadStream(MaxUploadSize * 1024 * 1024).CopyToAsync(tempFileStream);
                tempFileStream.Position = 0; // Reset stream before resizing

                // Await the resize result
                await using var resizedStream = await ImageProcessor.ResizeImageAsync(tempFileStream, 300, 0, ".webp", CancellationToken.None);

                // Create base64 thumbnail from resized stream
                resizedStream.Position = 0;
                tempSmallImage = await ToBase64ImageAsync(resizedStream, "image/webp");

                // Reset position and generate alt text
                resizedStream.Position = 0;
            }
            catch (OperationCanceledException)
            {
                // Alt text generation was cancelled by user input
            }
            finally
            {
                await ModelChanged.InvokeAsync(Model);
                Model.ThumbnailUrl = tempSmallImage;
                StateHasChanged();
            }
        }


        public static async Task<string> ToBase64ImageAsync(Stream imageStream, string contentType)
        {
            using var ms = new MemoryStream();
            await imageStream.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();

            return $"data:{contentType};base64,{Convert.ToBase64String(bytes)}";
        }

        private async Task<string> UploadTempFileInChunksAsync(IBrowserFile file, CancellationToken cancellationToken = default)
        {
            string randomFileName = Path.GetRandomFileName();

            string fileNameWithExtension = Path.ChangeExtension(randomFileName, Path.GetExtension(file.Name));

            string tempFilePath = Path.Combine(Path.GetTempPath(), fileNameWithExtension);

            await using (var tempFileStream = File.Create(tempFilePath))
            {
                // Read the browser file in chunks and write to temp file
                byte[] buffer = new byte[81920]; // 80KB buffer
                await using var inputStream = file.OpenReadStream(MaxUploadSize * 1024 * 1024, cancellationToken);
                int bytesRead;
                double totalRead = 0;
                long totalLength = file.Size;
                do
                {
                    bytesRead = await inputStream.ReadAsync(buffer, cancellationToken);
                    if (bytesRead > 0)
                    {
                        await tempFileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                        totalRead += bytesRead;
                        uploadProgress = totalLength > 0 ? totalRead / totalLength * 100 : 0;
                        await InvokeAsync(StateHasChanged);
                    }
                } while (bytesRead > 0);
            }

            return tempFilePath;
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