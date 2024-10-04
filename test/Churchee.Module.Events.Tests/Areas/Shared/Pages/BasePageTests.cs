using Bunit;
using Bunit.TestDoubles;
using Churchee.Common.Abstractions.Auth;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Radzen;

namespace Churchee.Module.Events.Tests.Areas.Shared.Pages
{
    public abstract class BasePageTests : TestContext
    {
        protected Mock<ICurrentUser> MockCurrentUser;
        protected Mock<NotificationService> MockNotificationService;
        protected Mock<IMediator> MockMediator;
        protected Mock<IConfiguration> MockConfiguration;

        protected BasePageTests()
        {
            // Common setup for all tests
            JSInterop.Mode = JSRuntimeMode.Loose;

            MockCurrentUser = new Mock<ICurrentUser>();
            Services.AddSingleton(MockCurrentUser.Object);

            MockNotificationService = new Mock<NotificationService>();
            Services.AddSingleton(MockNotificationService.Object);

            MockMediator = new Mock<IMediator>();
            Services.AddSingleton(MockMediator.Object);

            MockConfiguration = new Mock<IConfiguration>();
            Services.AddSingleton(MockConfiguration.Object);


            Services.AddRadzenComponents();

        }

        protected void SetInitialUrl<TComponent>() where TComponent : IComponent
        {
            var pageAttribute = typeof(TComponent).GetCustomAttributes(typeof(RouteAttribute), true).FirstOrDefault() as RouteAttribute;
            if (pageAttribute != null)
            {
                var navMan = Services.GetRequiredService<FakeNavigationManager>();

                navMan.NavigateTo(pageAttribute.Template, false);
            }
        }
    }
}
