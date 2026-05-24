using Churchee.CQRS.Abstractions;
using Churchee.CQRS.Resgistration;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Reflection;

namespace Churchee.CQRS.Tests.Resgistration
{
    // Tests exercise the public AddDispatcher registration surface by
    // providing handler types in the test assembly and then resolving
    // ISender/IPublisher from the built service provider.
    public class DispatcherRegistrationTests
    {
        // Simple request + handler pair discovered by AddDispatcher scanning.
        public sealed class TestRequest : IRequest<string> { }

        public sealed class TestRequestHandler : IRequestHandler<TestRequest, string>
        {
            public Task<string> Handle(TestRequest request, CancellationToken cancellationToken)
                => Task.FromResult("ok");
        }

        // Simple notification + handler which increments a counter via injected observer.
        public sealed class NotificationObserver
        {
            public int Count;
        }

        public sealed class TestNotification : INotification { }

        public sealed class TestNotificationHandler : INotificationHandler<TestNotification>
        {
            private readonly NotificationObserver _observer;
            public TestNotificationHandler(NotificationObserver observer) => _observer = observer;
            public Task Handle(TestNotification notification, CancellationToken cancellationToken)
            {
                Interlocked.Increment(ref _observer.Count);
                return Task.CompletedTask;
            }
        }

        // A pipeline behavior that prefixes the handler result so we can assert it ran.
        public sealed class PrefixBehavior : IPipelineBehavior<TestRequest, string>
        {
            private readonly string _prefix;
            public PrefixBehavior() => _prefix = "pref-";
            public async Task<string> Handle(TestRequest request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken)
            {
                var inner = await next();
                return _prefix + inner;
            }
        }

        [Fact]
        public async Task AddDispatcher_RegistersSender_RequestHandled()
        {
            var services = new ServiceCollection();

            // Register types needed by handlers (none in this test).
            // Scan the current test assembly for handlers and register dispatcher.
            services.AddDispatcher([Assembly.GetExecutingAssembly()]);

            var provider = services.BuildServiceProvider();

            var sender = provider.GetRequiredService<ISender>();

            var response = await sender.Send<string>(new TestRequest());

            response.Should().Be("ok");
        }

        [Fact]
        public async Task AddDispatcher_RegistersPublisher_NotificationHandlerInvoked()
        {
            var services = new ServiceCollection();

            // Provide an observer that the notification handler will increment.
            var observer = new NotificationObserver();
            services.AddSingleton(observer);

            services.AddDispatcher([Assembly.GetExecutingAssembly()]);

            var provider = services.BuildServiceProvider();

            var publisher = provider.GetRequiredService<IPublisher>();

            await publisher.Publish(new TestNotification());

            observer.Count.Should().Be(1);
        }

        [Fact]
        public async Task AddDispatcher_ResolvesPipelineBehaviors_BehaviorWrapsHandler()
        {
            var services = new ServiceCollection();

            // Register a pipeline behavior that modifies the handler result.
            services.AddTransient<IPipelineBehavior<TestRequest, string>, PrefixBehavior>();

            services.AddDispatcher([Assembly.GetExecutingAssembly()]);

            var provider = services.BuildServiceProvider();

            var sender = provider.GetRequiredService<ISender>();

            var response = await sender.Send<string>(new TestRequest());

            response.Should().Be("pref-ok");
        }

        [Fact]
        public async Task AddDispatcher_AllowsMockedBehavior_MoqCanBeUsed()
        {
            var services = new ServiceCollection();

            // Demonstrate using Moq: create a mock behavior that calls next and prefixes.
            var mock = new Mock<IPipelineBehavior<TestRequest, string>>();
            mock.Setup(m => m.Handle(It.IsAny<TestRequest>(), It.IsAny<RequestHandlerDelegate<string>>(), It.IsAny<CancellationToken>()))
                .Returns(async (TestRequest req, RequestHandlerDelegate<string> next, CancellationToken ct) =>
                {
                    var inner = await next();
                    return "mock-" + inner;
                });

            // Register the mock instance
            services.AddTransient(_ => mock.Object);

            services.AddDispatcher([Assembly.GetExecutingAssembly()]);

            var provider = services.BuildServiceProvider();

            var sender = provider.GetRequiredService<ISender>();

            var response = await sender.Send<string>(new TestRequest());

            response.Should().Be("mock-ok");

            mock.Verify(m => m.Handle(It.IsAny<TestRequest>(), It.IsAny<RequestHandlerDelegate<string>>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}