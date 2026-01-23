using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Tenancy.Entities;
using Churchee.Module.Videos.Helpers;
using MediatR;

namespace Churchee.Module.Videos.Features.Commands
{
    public class VideosEnabledCommandHandler : IRequestHandler<VideosEnabledCommand, CommandResponse>
    {

        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;
        private readonly ISettingStore _settingStore;

        public VideosEnabledCommandHandler(IDataStore dataStore, ICurrentUser currentUser, ISettingStore settingStore)
        {
            _dataStore = dataStore;
            _currentUser = currentUser;
            _settingStore = settingStore;
        }

        public async Task<CommandResponse> Handle(VideosEnabledCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            Guid listingTypeId = Guid.NewGuid();

            Guid detailPageTypeId = Guid.NewGuid();

            string pageNameForVideos = await _settingStore.GetSettingValue(Settings.VideosNameId, applicationTenantId);

            if (string.IsNullOrEmpty(pageNameForVideos))
            {
                await _settingStore.AddSetting(Settings.VideosNameId, applicationTenantId, "Name for videos. i.e videos/watch/player", request.PageNameForVideos);

                pageNameForVideos = request.PageNameForVideos;
            }

            await CreatePageTypes(applicationTenantId, listingTypeId, detailPageTypeId, cancellationToken);

            var tenant = await _dataStore.GetRepository<ApplicationTenant>().GetByIdAsync(applicationTenantId, cancellationToken);

            await CreateVideoListingPage(request, tenant, listingTypeId, cancellationToken);

            CreateViewTemplates(pageNameForVideos, applicationTenantId);

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }

        private void CreateViewTemplates(string pageName, Guid applicationTenantId)
        {
            var viewTemplatesRepo = _dataStore.GetRepository<ViewTemplate>();

            if (!viewTemplatesRepo.GetQueryable().Any(a => a.Location == "/Views/Shared/VideoListing.cshtml"))
            {
                var videoListing = new ViewTemplate(applicationTenantId, "/Views/Shared/VideoListing.cshtml", "");

                videoListing.SetContent($@"

                    @{{
                        ViewData[""Title""] = Model.Title;
                        
                        var page = ViewBag.Page; 
                    }}

                    <section>
                        <div class=""container mb-5"">
                            <h1>{pageName}</h1>
                            @await Component.InvokeAsync(""VideoListing"", new {{page = page}})
                        </div>
                    <section>

                ");

                viewTemplatesRepo.Create(videoListing);
            }

            if (!viewTemplatesRepo.GetQueryable().Any(a => a.Location == "/Views/Shared/VideoDetail.cshtml"))
            {
                var videoDetail = new ViewTemplate(applicationTenantId, "/Views/Shared/VideoDetail.cshtml", "");

                videoDetail.SetContent($@"

                    @{{
                        ViewData[""Title""] = Model.Title;
                        
                        var page = ViewBag.Page; 
                    }}

                    <section>
                        <div class=""container mb-5"">
                            <h1>{pageName}</h1>
                            @await Component.InvokeAsync(""VideoDetail"", new {{path = Context.Request.Path.ToString()}})
                        </div>
                    <section>

                ");

                viewTemplatesRepo.Create(videoDetail);
            }
        }

        private async Task CreateVideoListingPage(VideosEnabledCommand request, ApplicationTenant tenant, Guid listingPageTypeId, CancellationToken cancellationToken)
        {
            var pageRepo = _dataStore.GetRepository<Page>();

            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            string pageUrl = $"/{request.PageNameForVideos.ToURL()}";

            if (!pageRepo.GetQueryable().Any(a => a.Url == pageUrl))
            {
                var newListingPage = new Page(applicationTenantId, request.PageNameForVideos, pageUrl, $"Videos from {tenant.Name}", listingPageTypeId, null, false);

                pageRepo.Create(newListingPage);

                await _dataStore.SaveChangesAsync(cancellationToken);
            }
        }

        private async Task CreatePageTypes(Guid applicationTenantId, Guid listingPageTypeId, Guid detailsPageTypeId, CancellationToken cancellationToken)
        {
            var pageTypeRepo = _dataStore.GetRepository<PageType>();

            var alreadyExists = pageTypeRepo.AnyWithFiltersDisabled(w => w.SystemKey == PageTypes.VideoDetailPageTypeId && w.ApplicationTenantId == applicationTenantId);

            if (!alreadyExists)
            {

                var newDetailPageType = new PageType(detailsPageTypeId, PageTypes.VideoDetailPageTypeId, applicationTenantId, false, "Video Detail", false);

                var newListingPageType = new PageType(listingPageTypeId, PageTypes.VideoListingPageTypeId, applicationTenantId, true, "Video Listing", false);

                newListingPageType.ChildrenTypes.Add(new PageTypeTypeMapping() { ParentPageTypeId = listingPageTypeId, ChildPageTypeId = detailsPageTypeId });

                pageTypeRepo.Create(newDetailPageType);

                pageTypeRepo.Create(newListingPageType);

                await _dataStore.SaveChangesAsync(cancellationToken);
            }

            await CreateListingComponentTemplate(applicationTenantId, cancellationToken);
            await CreateDetailComponentTemplate(applicationTenantId, cancellationToken);

        }

        private async Task CreateListingComponentTemplate(Guid applicationTenantId, CancellationToken cancellationToken)
        {
            var viewTemplateRepo = _dataStore.GetRepository<ViewTemplate>();

            if (!viewTemplateRepo.GetQueryable().Any(a => a.Location == "/Views/Shared/Components/VideoListing/Default.cshtml"))
            {
                var videoListingComponent = new ViewTemplate(applicationTenantId, "/Views/Shared/Components/VideoListing/Default.cshtml", "");

                videoListingComponent.SetContent($@"
 
                            
@model VideoListingComponentModel

        <div class=""row row-cols-1 row-cols-md-2 row-cols-lg-3 row-cols-xxl-4 g-3 mb-5"">" +

            @"@foreach (var item in Model.Items){

                <div class=""col"">
                    <div class=""card shadow-sm"" itemscope itemtype=""https://schema.org/VideoObject"">

                            <a itemprop=""url"" href=""@item.Url"">

                            <div class=""ratio ratio-16x9"">
                                <img class=""card-img-top card-img-video"" src=""@item.Thumbnail"">
                            </div>

                            </a>
                    
                        <div class=""card-body"">
                            <p class=""card-text"" itemprop=""name"">@item.Title</p>
                            <div class=""d-flex justify-content-between align-items-center"">
                            <div class=""btn-group"">
                                <a itemprop = ""url"" class=""btn btn-sm btn-outline-secondary"" href=""@item.Url"">Details</a>
                            </div>
                            <small class=""text-body-secondary""><time itemprop = ""datePublished"" datetime=""@item.PublishedDate.ToShortDateString()"">@item.PublishedDate.ToShortDateString()</time></small>
                            </div>
                        </div>
                    </div>
                </div>

            }" +


            @"</div>" +

 @"<nav aria-label=""Page navigation example"">
    <ul class=""pagination justify-content-center"">" +

        @"@if(Model.Page > 1)
        {
            <li class=""page-item"">
              <a class=""page-link"" href=""?page=@(Model.Page - 1)"">Previous</a>
            </li>

            @for(var i = 1; i < Model.Page; i ++)
            {
                <li class=""page-item""><a class=""page-link"" href=""?page=@i"">@i</a></li>
            }

        }
        else{
            <li class=""page-item disabled"">
                    <a class=""page-link"" href="""">Previous</a>
            </li>
        }" +

        @"<li class=""page-item""><a class=""page-link"" href=""?page=@(Model.Page+1)"">Next</a></li>

    </ul>
</nav>" +
                        "");


                viewTemplateRepo.Create(videoListingComponent);

                await _dataStore.SaveChangesAsync(cancellationToken);

            }
        }

        private async Task CreateDetailComponentTemplate(Guid applicationTenantId, CancellationToken cancellationToken)
        {
            var viewTemplateRepo = _dataStore.GetRepository<ViewTemplate>();

            if (!viewTemplateRepo.GetQueryable().Any(a => a.Location == "/Views/Shared/Components/VideoDetail/Default.cshtml"))
            {
                var videoListingComponent = new ViewTemplate(applicationTenantId, "/Views/Shared/Components/VideoDetail/Default.cshtml", "");

                videoListingComponent.SetContent($@"
 
@model VideoDetailComponentModel


      <div class=""container-fluid py-5"">

        <div class=""ratio ratio-16x9"">
            <iframe src=""@Model.EmbedUri"" title=""YouTube video player"" frameborder=""0"" allow=""accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share"" referrerpolicy=""strict-origin-when-cross-origin"" allowfullscreen></iframe>
        </div>

    <div class=""row overflow-hidden flex-md-row h-md-250 position-relative mb-5"">
            <div class=""col p-4 d-flex flex-column position-static"">
              <h2 class=""mb-0"">@Model.Title</h2>
              
              <div class=""mb-4 text-body-secondary"">
                  <small><em>Published: @Model.PublishedDate.ToShortDateString()</em></small>
              </div>


            </div>
                <div class=""col-auto d-none d-lg-block"">
                <img src=""@Model.Thumbnail"" style=""width:300px""/>
                </div>
          </div>

    </div>

                ");


                viewTemplateRepo.Create(videoListingComponent);

                await _dataStore.SaveChangesAsync(cancellationToken);

            }
        }

    }
}
