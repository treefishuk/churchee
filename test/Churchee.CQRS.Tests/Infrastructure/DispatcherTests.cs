using Churchee.CQRS.Abstractions;
using Churchee.CQRS.Infrastructure;
using Churchee.Test.Helpers.Validation;
using Moq;
using System.Collections.Frozen;

namespace Churchee.CQRS.Tests.Infrastructure
{
    public class DispatcherTests
    {
        private sealed class TestRequest : IRequest<string> { }

        private sealed class TestRequestHandler : IRequestHandlerBase<string>
        {
            private readonly string _result;
            public TestRequestHandler(string result) => _result = result;

            public Task<string> Handle(IRequest<string> request, IServiceProvider provider, CancellationToken cancellationToken)
                => Task.FromResult(_result);
        }

        private sealed class TestNotification : INotification { }

        private sealed class TestNotificationHandler : INotificationHandlerBase
        {
            public bool WasCalled { get; private set; }

            public Task Handle(object notification, IServiceProvider provider, CancellationToken cancellationToken)
            {
                WasCalled = true;
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task Send_NullRequest_ThrowsArgumentNullException()
        {
            var reqMap = FrozenDictionary.Create<Type, IRequestHandlerBase>([]);
            var notMap = FrozenDictionary.Create<Type, INotificationHandlerBase>([]);
            var registry = new DispatcherRegistry(reqMap, notMap);
            var dispatcher = new Dispatcher(Mock.Of<IServiceProvider>(), registry);

            IRequest<string>? request = null;

            var act = async () => await dispatcher.Send(request!);

            var exception = await Assert.ThrowsAsync<ArgumentNullException>(act);
        }

        [Fact]
        public async Task Send_NoHandlerRegistered_ThrowsInvalidOperationException()
        {
            var reqMap = FrozenDictionary.Create<Type, IRequestHandlerBase>([]);
            var notMap = FrozenDictionary.Create<Type, INotificationHandlerBase>([]);
            var registry = new DispatcherRegistry(reqMap, notMap);
            var dispatcher = new Dispatcher(Mock.Of<IServiceProvider>(), registry);

            var request = new TestRequest();

            var act = async () => await dispatcher.Send(request);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(act);

            exception.Message.Should().Be($"No handler registered for request type '{request.GetType().FullName}'.");
        }

        [Fact]
        public async Task Send_HandlerRegistered_ReturnsResponse()
        {
            var request = new TestRequest();
            var handler = new TestRequestHandler("ok");

            var reqMap = FrozenDictionary.Create(new[]
            {
                new KeyValuePair<Type, IRequestHandlerBase>(request.GetType(), handler)
            });

            var notMap = FrozenDictionary.Create<Type, INotificationHandlerBase>([]);
            var registry = new DispatcherRegistry(reqMap, notMap);
            var dispatcher = new Dispatcher(Mock.Of<IServiceProvider>(), registry);

            string result = await dispatcher.Send(request);

            result.Should().Be("ok");
        }

        [Fact]
        public async Task Publish_NullNotification_ThrowsArgumentNullException()
        {
            var reqMap = FrozenDictionary.Create<Type, IRequestHandlerBase>([]);
            var notMap = FrozenDictionary.Create<Type, INotificationHandlerBase>([]);
            var registry = new DispatcherRegistry(reqMap, notMap);
            var dispatcher = new Dispatcher(Mock.Of<IServiceProvider>(), registry);

            TestNotification? notification = null;

            var act = async () => await dispatcher.Publish<TestNotification>(notification!);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task Publish_NoHandlers_CompletesImmediately()
        {
            var reqMap = FrozenDictionary.Create<Type, IRequestHandlerBase>([]);
            var notMap = FrozenDictionary.Create<Type, INotificationHandlerBase>([]);
            var registry = new DispatcherRegistry(reqMap, notMap);
            var dispatcher = new Dispatcher(Mock.Of<IServiceProvider>(), registry);

            var notification = new TestNotification();

            var task = dispatcher.Publish(notification);
            await task; // should complete without exception

            task.IsCompletedSuccessfully.Should().BeTrue();
        }

        [Fact]
        public async Task Publish_HandlerRegistered_InvokesHandler()
        {
            var notification = new TestNotification();
            var handler = new TestNotificationHandler();

            var notMap = FrozenDictionary.Create(new[]
            {
                new KeyValuePair<Type, INotificationHandlerBase>(notification.GetType(), handler)
            });

            var reqMap = FrozenDictionary.Create<Type, IRequestHandlerBase>([]);
            var registry = new DispatcherRegistry(reqMap, notMap);
            var dispatcher = new Dispatcher(Mock.Of<IServiceProvider>(), registry);

            await dispatcher.Publish(notification);

            handler.WasCalled.Should().BeTrue();
        }
    }
}