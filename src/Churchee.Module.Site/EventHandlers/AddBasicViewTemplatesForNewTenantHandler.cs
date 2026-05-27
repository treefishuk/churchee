using Churchee.Common.Storage;
using Churchee.CQRS.Abstractions;
using Churchee.Module.Site.Entities;
using Churchee.Module.Tenancy.Entities;
using Churchee.Module.Tenancy.Events;

namespace Churchee.Module.Site.EventHandlers
{
    public class AddBasicViewTemplatesForNewTenantHandler : INotificationHandler<TenantAddedEvent>
    {

        private readonly IDataStore _dataStore;

        public AddBasicViewTemplatesForNewTenantHandler(IDataStore store)
        {
            _dataStore = store;
        }

        public async Task Handle(TenantAddedEvent notification, CancellationToken cancellationToken)
        {
            var applicationTenantId = notification.ApplicationTenantId;

            var tenant = await _dataStore.GetRepository<ApplicationTenant>().GetByIdAsync(notification.ApplicationTenantId, cancellationToken);

            var repo = _dataStore.GetRepository<ViewTemplate>();

            repo.Create(new ViewTemplate(applicationTenantId, "/Views/Shared/_Layout.cshtml", GetLayoutContent(tenant.Name, tenant.DevName)));
            repo.Create(new ViewTemplate(applicationTenantId, "/Views/Shared/_ViewImports.cshtml", ImportsContent));
            repo.Create(new ViewTemplate(applicationTenantId, "/Views/Shared/_ViewStart.cshtml", ViewStartContent));

            await _dataStore.SaveChangesAsync(cancellationToken);
        }

        private static string ViewStartContent => """"
            @{
                Layout = "_Layout";
            }
        """";

        private static string ImportsContent => """"
            @using Churchee.Sites
            @using Churchee.Sites.Models
            @using Churchee.Sites.Services
            @addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
        """";

        private static string GetLayoutContent(string tenantName, string tenantDevName) => $""""

            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="utf-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <meta name="description" content="@Model.Description">
                <meta name="application-name" content="{tenantName}"> 
                @RenderSection("MetaInfo", required: false)
                <title>@Model.Title | {tenantName}</title>
                <link rel="preconnect" href="https://cdn.jsdelivr.net" crossorigin>
                <link rel="preconnect" href="https://{tenantDevName}.churchee.net" crossorigin>   
                <link rel="icon" href="https://{tenantDevName}.churchee.net/favicons/favicon.ico">
                <link rel="apple-touch-icon" sizes="180x180" href="https://{tenantDevName}.churchee.net/favicons/apple-touch-icon.png">
                <link rel="icon" type="image/png" href="https://{tenantDevName}.churchee.net/favicons/favicon-32x32.png">
                <link rel="icon" type="image/png" sizes="32x32" href="https://{tenantDevName}.churchee.net/favicons/favicon-32x32.png">
                <link rel="icon" type="image/png" sizes="16x16" href="https://{tenantDevName}.churchee.net/favicons/favicon-16x16.png">   
                @RenderSection("PageStyles", required: false)
                <link rel="stylesheet" href="~/css/site.min.css">
            </head>
            <body>
                <header>
                        @await Component.InvokeAsync("Menu")
                </header>
                <main>
                    @RenderBody()
                </main>
            </body>
            <footer class="footer">
            </footer>
        """";

    }
}
