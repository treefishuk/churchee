using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using MediatR;

namespace Churchee.Module.Site.Features.Blog.Commands
{
    public class SeedDefaultArticlesListingIfNeededCommandHandler : IRequestHandler<SeedDefaultArticlesListingIfNeededCommand, CommandResponse>
    {
        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;

        public SeedDefaultArticlesListingIfNeededCommandHandler(IDataStore dataStore, ICurrentUser currentUser)
        {
            _dataStore = dataStore;
            _currentUser = currentUser;
        }

        public async Task<CommandResponse> Handle(SeedDefaultArticlesListingIfNeededCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            var pageTypeRepo = _dataStore.GetRepository<PageType>();
            var pageRepo = _dataStore.GetRepository<Page>();

            await CreateListingPageTypeIfItDoesNotExist(applicationTenantId, pageTypeRepo, cancellationToken);

            await CreateListingPageIfItDoesNotExist(applicationTenantId, pageTypeRepo, pageRepo, cancellationToken);

            await CreateDetailsPageTypeIfItDoesNotExist(applicationTenantId, pageTypeRepo, cancellationToken);

            if (!pageTypeRepo.AnyWithFiltersDisabled(a => a.ApplicationTenantId == applicationTenantId && a.SystemKey == Helpers.PageTypes.BlogDetailPageTypeId))
            {
                return new CommandResponse();
            }

            var articleDetail = new PageType(Guid.NewGuid(), Helpers.PageTypes.BlogDetailPageTypeId, applicationTenantId, false, "Blog Detail", false);

            return new CommandResponse();
        }

        private async Task CreateListingPageIfItDoesNotExist(Guid applicationTenantId, Common.Abstractions.Storage.IRepository<PageType> pageTypeRepo, Common.Abstractions.Storage.IRepository<Page> pageRepo, CancellationToken cancellationToken)
        {
            if (!pageRepo.AnyWithFiltersDisabled(x => x.PageType.SystemKey == Helpers.PageTypes.BlogListingPageTypeId && x.ApplicationTenantId == applicationTenantId))
            {

                var listingPageType = await pageTypeRepo.FirstOrDefaultAsync(new PageTypeFromSystemKeySpecification(Helpers.PageTypes.BlogListingPageTypeId, applicationTenantId), cancellationToken: cancellationToken);

                var listingPage = new Page(applicationTenantId, "Blog", "/blog", "Blog", listingPageType.Id, null, false)
                {
                    IsSystem = true
                };

                pageRepo.Create(listingPage);

                await _dataStore.SaveChangesAsync(cancellationToken);

            }
        }

        private async Task CreateListingPageTypeIfItDoesNotExist(Guid applicationTenantId, Common.Abstractions.Storage.IRepository<PageType> pageTypeRepo, CancellationToken cancellationToken)
        {
            if (!pageTypeRepo.AnyWithFiltersDisabled(a => a.ApplicationTenantId == applicationTenantId && a.SystemKey == Helpers.PageTypes.BlogListingPageTypeId))
            {
                var articleListing = new PageType(Guid.NewGuid(), Helpers.PageTypes.BlogListingPageTypeId, applicationTenantId, true, "Blog Listing");

                pageTypeRepo.Create(articleListing);

                await _dataStore.SaveChangesAsync(cancellationToken);

            }
        }

        private async Task CreateDetailsPageTypeIfItDoesNotExist(Guid applicationTenantId, Common.Abstractions.Storage.IRepository<PageType> pageTypeRepo, CancellationToken cancellationToken)
        {
            if (!pageTypeRepo.AnyWithFiltersDisabled(a => a.ApplicationTenantId == applicationTenantId && a.SystemKey == Helpers.PageTypes.BlogDetailPageTypeId))
            {
                var articleDetail = new PageType(Guid.NewGuid(), Helpers.PageTypes.BlogDetailPageTypeId, applicationTenantId, true, "Blog Detail");

                pageTypeRepo.Create(articleDetail);

                await _dataStore.SaveChangesAsync(cancellationToken);

            }
        }
    }
}
