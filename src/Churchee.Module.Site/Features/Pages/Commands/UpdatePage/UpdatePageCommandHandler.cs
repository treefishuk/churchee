using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Common.ValueTypes;
using Churchee.CQRS.Abstractions;
using Churchee.ImageProcessing.Jobs;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using System.Text.Json;

namespace Churchee.Module.Site.Features.Pages.Commands.UpdatePage
{
    public class UpdatePageCommandHandler : IRequestHandler<UpdatePageCommand, CommandResponse>
    {

        private readonly IDataStore _storage;
        private readonly IImageProcessor _imageProcessor;
        private readonly IJobService _jobService;
        private readonly ICurrentUser _currentUser;

        public UpdatePageCommandHandler(IDataStore storage, IImageProcessor imageProcessor, IJobService jobService, ICurrentUser currentUser)
        {
            _storage = storage;
            _imageProcessor = imageProcessor;
            _jobService = jobService;
            _currentUser = currentUser;
        }

        public async Task<CommandResponse> Handle(UpdatePageCommand request, CancellationToken cancellationToken)
        {
            var page = _storage.GetRepository<Page>()
                .ApplySpecification(new PageWithContentAndPropertiesSpecification(request.PageId))
                .First();

            page.UpdateInfo(request.Title, request.Description, request.ParentId, request.Order);

            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            await ProcessImages(request, applicationTenantId, page, cancellationToken);

            await UpdateContent(request.Content, page, applicationTenantId, cancellationToken);

            await _storage.SaveChangesAsync(cancellationToken);

            if (request.Publish)
            {
                page.Publish();
            }

            if (request.Unpublish)
            {
                page.Unpublish();
            }

            await _storage.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }

        private async Task ProcessImages(UpdatePageCommand request, Guid applicationTenantId, Page page, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.ImageTempPath) || string.IsNullOrEmpty(request.ImageFileName))
            {
                return;
            }

            string imagePath = await _imageProcessor.ConvertTempImageToFullImage(request.ImageTempPath, request.ImageFileName, "img/pages", applicationTenantId, cancellationToken);

            if (string.IsNullOrEmpty(imagePath))
            {
                return;
            }

            _jobService.QueueJob<ImageCropsGenerator>(x => x.CreateCropsAsync(applicationTenantId, imagePath, true, CancellationToken.None));

            page.SetImageUrl(imagePath.Replace(".webp", ""));
        }

        public async Task UpdateContent(List<KeyValuePair<Guid, string>> content, Page page, Guid applicationTenantId, CancellationToken cancellationToken)
        {
            if (content == null || content.Count == 0)
            {
                return;
            }

            foreach (var item in content)
            {
                var pageContent = page.PageContent.FirstOrDefault(d => d.PageTypeContentId == item.Key);

                if (pageContent != null && pageContent.PageTypeContent.Type == "Image")
                {
                    var imageData = string.IsNullOrEmpty(pageContent.Value) ? new ImageSimple() : JsonSerializer.Deserialize<ImageSimple>(pageContent.Value);

                    var newImage = JsonSerializer.Deserialize<ImageSimple>(item.Value);

                    if (!string.IsNullOrEmpty(newImage.TempUrl))
                    {
                        string finalUrl = await _imageProcessor.ConvertTempImageToFullImage(newImage.TempUrl, newImage.Url, "/img/pages", applicationTenantId, cancellationToken);

                        imageData.Url = finalUrl.Replace(".webp", "");

                        imageData.TempUrl = string.Empty;

                        _jobService.QueueJob<ImageCropsGenerator>(x => x.CreateCropsAsync(applicationTenantId, finalUrl, true, CancellationToken.None));
                    }

                    imageData.AltText = newImage.AltText;

                    pageContent.Value = JsonSerializer.Serialize(imageData);
                }

                else if (pageContent != null)
                {
                    pageContent.Value = item.Value;
                }

                pageContent.IncrementVersion();

            }

        }


    }
}
