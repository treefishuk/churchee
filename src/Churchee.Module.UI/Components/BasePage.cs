using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Radzen;

namespace Churchee.Module.UI.Components
{
    public class BasePage : ComponentBase
    {
        [Inject]
        protected ILogger<BasePage> Logger { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected NotificationService NotificationService { get; set; } = default!;

        [Inject]
        protected IMediator Mediator { get; set; } = default!;

    }
}
