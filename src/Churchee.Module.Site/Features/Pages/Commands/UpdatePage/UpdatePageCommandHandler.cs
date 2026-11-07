using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Common.ValueTypes;
using Churchee.CQRS.Abstractions;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using Hangfire;
using System.Text.Json;

namespace Churchee.Module.Site.Features.Pages.Commands.UpdatePage
{
    public class UpdatePageCommandHandler : IRequestHandler<UpdatePageCommand, CommandResponse>
    {

        private readonly IDataStore _storage;
        private readonly IImageProcessor _imageProcessor;
        private readonly IJobService _jobService;
        private readonly ICurrentUser _currentUser;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public UpdatePageCommandHandler(IDataStore storage, IImageProcessor imageProcessor, IJobService jobService, ICurrentUser currentUser, IBackgroundJobClient backgroundJobClient)
        {
            _storage = storage;
            _imageProcessor = imageProcessor;
            _jobService = jobService;
            _currentUser = currentUser;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<CommandResponse> Handle(UpdatePageCommand request, CancellationToken cancellationToken)
        {
            var page = _storage.GetRepository<Page>()
                .ApplySpecification(new PageWithContentAndPropertiesSpecification(request.PageId))
                .First();

            page.UpdateInfo(request.Title, request.Description, request.ParentId, request.Order);

            page.SetImageInfo(request.ImageFileName, string.Empty);

            var applicationTenantId = await _currentUser.GetApplicationTenantId();

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
                    await CreateFinalImagesForPageContent(applicationTenantId, item, pageContent, cancellationToken);
                }

                else
                {
                    pageContent?.Value = item.Value;
                }

                pageContent?.IncrementVersion();

            }

        }

        private async Task CreateFinalImagesForPageContent(Guid applicationTenantId, KeyValuePair<Guid, string> item, PageContent pageContent, CancellationToken cancellationToken)
        {
            var imageData = string.IsNullOrEmpty(pageContent.Value) ? new ImageSimple() : JsonSerializer.Deserialize<ImageSimple>(pageContent.Value);

            var newImage = JsonSerializer.Deserialize<ImageSimple>(item.Value);

            imageData.Url = newImage.Url.GetFullPathWithoutExtension();

            imageData.AltText = newImage.AltText;

            pageContent.Value = JsonSerializer.Serialize(imageData);
        }
    }
}
