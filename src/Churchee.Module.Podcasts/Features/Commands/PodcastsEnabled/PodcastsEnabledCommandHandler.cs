using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Tenancy.Entities;
using MediatR;

namespace Churchee.Module.Podcasts.Features.Commands
{
    public class PodcastsEnabledCommandHandler : IRequestHandler<PodcastsEnabledCommand, CommandResponse>
    {

        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;
        private readonly Guid _podcastListingPageTypeId = Guid.Parse("c985a344-f201-4949-b609-bb66144d81a6");
        private readonly Guid _podcastDetailPageTypeId = Guid.Parse("f88412e5-9647-4232-8389-4edf685ecf4e");
        private readonly Guid _podcastsNameId = Guid.Parse("4379e3d3-fa40-489b-b80d-01c30835fa9d");
        private readonly ISettingStore _settingStore;

        public PodcastsEnabledCommandHandler(IDataStore dataStore, ICurrentUser currentUser, ISettingStore settingStore)
        {
            _dataStore = dataStore;
            _currentUser = currentUser;
            _settingStore = settingStore;
        }

        public async Task<CommandResponse> Handle(PodcastsEnabledCommand request, CancellationToken cancellationToken)
        {

            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            string pageNameForPodcasts = await _settingStore.GetSettingValue(_podcastsNameId, applicationTenantId);

            if (string.IsNullOrEmpty(pageNameForPodcasts))
            {
                await _settingStore.AddSetting(_podcastsNameId, applicationTenantId, "Name for podcasts. i.e sermons/talks/podcasts", request.PageNameForPodcasts);

                pageNameForPodcasts = request.PageNameForPodcasts;
            }

            await CreatePageTypes(applicationTenantId);

            var tenant = _dataStore.GetRepository<ApplicationTenant>().GetById(applicationTenantId);

            await CreatePodcastsListingPage(request, tenant);

            CreateViewTemplates(pageNameForPodcasts, applicationTenantId);

            await _dataStore.SaveChangesAsync();

            return new CommandResponse();
        }

        private void CreateViewTemplates(string pageName, Guid applicationTenantId)
        {
            var viewTemplatesRepo = _dataStore.GetRepository<ViewTemplate>();

            if (!viewTemplatesRepo.GetQueryable().Any(a => a.Location == "/Views/Shared/PodcastListing.cshtml"))
            {
                var podcastListing = new ViewTemplate(applicationTenantId, "/Views/Shared/PodcastListing.cshtml", "");

                podcastListing.SetContent($@"

                    @{{
                        ViewData[""Title""] = Model.Title;
                        
                        var page = ViewBag.Page; 
                    }}

                    <section>
                        <div class=""container mb-5"">
                            <h1>{pageName}</h1>
                            @await Component.InvokeAsync(""PodcastListing"", new {{page = page}})
                        </div>
                    <section>



                ");

                viewTemplatesRepo.Create(podcastListing);

            }

            if (!viewTemplatesRepo.GetQueryable().Any(a => a.Location == "/Views/Shared/PodcastDetail.cshtml"))
            {

                var podcastDetail = new ViewTemplate(applicationTenantId, "/Views/Shared/PodcastDetail.cshtml", "");

                podcastDetail.SetContent($@"

                    @{{
                        ViewData[""Title""] = Model.Title;
                        
                        var page = ViewBag.Page; 
                    }}

                    <section>
                        <div class=""container mb-5"">
                            <h1>{pageName}</h1>
                            @await Component.InvokeAsync(""PodcastDetail"", new {{path = Context.Request.Path.ToString()}})
                        </div>
                    <section>

                ");

                viewTemplatesRepo.Create(podcastDetail);

            }
        }

        private async Task CreatePodcastsListingPage(PodcastsEnabledCommand request, ApplicationTenant tenant)
        {
            var pageRepo = _dataStore.GetRepository<Page>();

            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            string pageUrl = $"/{request.PageNameForPodcasts.ToURL()}";

            if (!pageRepo.GetQueryable().Any(a => a.Url == pageUrl))
            {
                var newListingPage = new Page(applicationTenantId, request.PageNameForPodcasts, pageUrl, $"Talks/Sermons/Podasts from {tenant.Name}", _podcastListingPageTypeId, null, false);

                pageRepo.Create(newListingPage);

                await _dataStore.SaveChangesAsync();
            }
        }


        private async Task CreatePageTypes(Guid applicationTenantId)
        {
            var pageTypeRepo = _dataStore.GetRepository<PageType>();

            if (pageTypeRepo.GetById(_podcastListingPageTypeId) == null)
            {
                var newDetailPageType = new PageType(_podcastDetailPageTypeId, applicationTenantId, false, "Podcast Detail", false);

                var newListingPageType = new PageType(_podcastListingPageTypeId, applicationTenantId, true, "Podcast Listing", false);

                newListingPageType.ChildrenTypes.Add(new WebContentTypeTypeMapping() { ParentWebContentTypeId = _podcastListingPageTypeId, ChildWebContentTypeId = _podcastDetailPageTypeId });

                pageTypeRepo.Create(newDetailPageType);

                pageTypeRepo.Create(newListingPageType);

                await _dataStore.SaveChangesAsync();
            }

            var viewTemplateRepo = _dataStore.GetRepository<ViewTemplate>();

            if (!viewTemplateRepo.GetQueryable().Any(a => a.Location == "/Views/Shared/Components/PodcastListing/Default.cshtml"))
            {
                var podcastListingComponant = new ViewTemplate(applicationTenantId, "/Views/Shared/Components/PodcastListing/Default.cshtml", "");

                podcastListingComponant.SetContent($@"
 
                            
@model PodcastListingComponantModel

        <div class=""row row-cols-1 row-cols-md-2 row-cols-lg-3 row-cols-xxl-4 g-3 mb-5"">" +

            @"@foreach (var item in Model.Items){

                <div class=""col"" itemscope itemtype=""https://schema.org/PodcastEpisode"">
                    <div class=""card shadow-sm"">

                            <a itemprop=""url"" href=""@item.Url""><img class=""bd-placeholder-img card-img-top"" src=""@item.Thumbnail""></a>
                    
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


                viewTemplateRepo.Create(podcastListingComponant);

                await _dataStore.SaveChangesAsync();

            }



        }
    }
}
