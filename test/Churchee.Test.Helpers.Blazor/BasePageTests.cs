using Bunit;
using Bunit.TestDoubles;
using Churchee.Common.Abstractions.Auth;
using Churchee.Module.UI.Models;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Radzen;

namespace Churchee.Test.Helpers.Blazor
{
    public abstract class BasePageTests : BunitContext
    {
        protected Mock<ICurrentUser> MockCurrentUser;
        protected CustomNotificationService NotificationService;
        protected TestDialogService DialogService;
        protected Mock<IMediator> MockMediator;
        protected Mock<IConfiguration> MockConfiguration;
        protected CurrentPage CurrentPage;
        protected ITenantResolver TenantResolver;

        protected BasePageTests()
        {
            // Common setup for all tests
            JSInterop.Mode = JSRuntimeMode.Loose;

            MockCurrentUser = new Mock<ICurrentUser>();
            Services.AddSingleton(MockCurrentUser.Object);

            DialogService = new TestDialogService(new BunitNavigationManager(this), JSInterop.JSRuntime);
            Services.AddSingleton<DialogService>(DialogService);

            NotificationService = new CustomNotificationService();
            Services.AddSingleton<NotificationService>(NotificationService);

            MockMediator = new Mock<IMediator>();
            Services.AddSingleton(MockMediator.Object);

            MockConfiguration = new Mock<IConfiguration>();
            Services.AddSingleton(MockConfiguration.Object);

            CurrentPage = new CurrentPage();
            Services.AddSingleton(CurrentPage);

            TenantResolver = new Mock<ITenantResolver>().Object;
            Services.AddSingleton(TenantResolver);

        }

        protected void SetInitialUrl<TComponent>() where TComponent : IComponent
        {
            if (typeof(TComponent).GetCustomAttributes(typeof(RouteAttribute), true).FirstOrDefault() is RouteAttribute pageAttribute)
            {
                var navMan = Services.GetRequiredService<BunitNavigationManager>();

                navMan.NavigateTo(pageAttribute.Template, false);
            }
        }

        public class CustomNotificationService : NotificationService
        {
            public List<NotificationMessage> Notifications { get; } = [];

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

        public Mock<IHttpContextAccessor> GetMockHttpContextAccessor()
        {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            return mockHttpContextAccessor;
        }

    }

}
