using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.CQRS.Abstractions;
using Churchee.ImageProcessing.Jobs;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

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
            page.UpdateContent(request.Content);


            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            await ProcessImage(request, applicationTenantId, page, cancellationToken);

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

        private async Task ProcessImage(UpdatePageCommand request, Guid applicationTenantId, Page page, CancellationToken cancellationToken)
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
    }
}
