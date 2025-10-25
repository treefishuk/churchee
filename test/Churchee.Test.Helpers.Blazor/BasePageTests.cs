using Bunit;
using Bunit.TestDoubles;
using Churchee.Common.Abstractions.Auth;
using Churchee.Module.UI.Models;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Radzen;

namespace Churchee.Test.Helpers.Blazor
{
    public abstract class BasePageTests : TestContext
    {
        protected Mock<ICurrentUser> MockCurrentUser;
        protected CustomNotificationService NotificationService;
        protected DialogService DialogService;
        protected Mock<IMediator> MockMediator;
        protected Mock<IConfiguration> MockConfiguration;
        protected CurrentPage CurrentPage;

        protected BasePageTests()
        {
            // Common setup for all tests
            JSInterop.Mode = JSRuntimeMode.Loose;

            MockCurrentUser = new Mock<ICurrentUser>();
            Services.AddSingleton(MockCurrentUser.Object);

            DialogService = new DialogService(new FakeNavigationManager(this), JSInterop.JSRuntime);
            Services.AddSingleton(DialogService);

            NotificationService = new CustomNotificationService();
            Services.AddSingleton<NotificationService>(NotificationService);

            MockMediator = new Mock<IMediator>();
            Services.AddSingleton(MockMediator.Object);

            MockConfiguration = new Mock<IConfiguration>();
            Services.AddSingleton(MockConfiguration.Object);

            CurrentPage = new CurrentPage();
            Services.AddSingleton(CurrentPage);

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

        public class CustomNotificationService : NotificationService
        {
            public List<NotificationMessage> Notifications { get; } = new List<NotificationMessage>();

            public CustomNotificationService()
            {
                Messages.CollectionChanged += OnMessagesChanged;
            }

            private void OnMessagesChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                if (e.NewItems != null)
                {
                    foreach (NotificationMessage newItem in e.NewItems)
                    {
                        Notifications.Add(newItem);
                    }
                }
            }
        }

    }

}
