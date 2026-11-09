using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Common.ValueTypes;
using Churchee.CQRS.Abstractions;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using System.Text.Json;

namespace Churchee.Module.Site.Features.Pages.Commands.UpdatePage
{
    public class UpdatePageCommandHandler : IRequestHandler<UpdatePageCommand, CommandResponse>
    {

        private readonly IDataStore _storage;
        private readonly ICurrentUser _currentUser;

        public UpdatePageCommandHandler(IDataStore storage, ICurrentUser currentUser)
        {
            _storage = storage;
            _currentUser = currentUser;
        }

        public async Task<CommandResponse> Handle(UpdatePageCommand request, CancellationToken cancellationToken)
        {
            var page = _storage.GetRepository<Page>()
                .ApplySpecification(new PageWithContentAndPropertiesSpecification(request.PageId))
                .First();

            page.UpdateInfo(request.Title, request.Description, request.ParentId, request.Order);

            page.SetImageInfo(string.IsNullOrEmpty(request.ImageFileName) ? page.ImageUrl : request.ImageFileName, request.ImageAltTag);

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
                    await CreateFinalImagesForPageContent(item, pageContent);
                }

                else
                {
                    pageContent?.Value = item.Value;
                }

                pageContent?.IncrementVersion();

            }

        }

        private static async Task CreateFinalImagesForPageContent(KeyValuePair<Guid, string> item, PageContent pageContent)
        {
            var imageData = string.IsNullOrEmpty(pageContent.Value) ? new ImageSimple() : JsonSerializer.Deserialize<ImageSimple>(pageContent.Value);

            var newImage = JsonSerializer.Deserialize<ImageSimple>(item.Value);

            imageData.Url = newImage.Url.GetFullPathWithoutExtension();

            imageData.AltText = newImage.AltText;

            pageContent.Value = JsonSerializer.Serialize(imageData);
        }
    }
}
