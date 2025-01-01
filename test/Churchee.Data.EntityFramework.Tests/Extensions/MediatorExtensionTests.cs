using Churchee.Common.Abstractions.Entities;
using Churchee.Data.EntityFramework.Extensions;
using Churchee.Module.Identity.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Churchee.Data.EntityFramework.Tests.Extensions
{
    public class MediatorExtensionTests
    {
        [Fact]
        public async Task DispatchDomainEventsAsync_ShouldDispatchDomainEvents()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var mediatorMock = new Mock<IMediator>();
            using var context = new ApplicationDbContext(options);

            var testEntity = new TestEntity();
            testEntity.AddDomainEvent(new TestDomainEvent());
            context.Add(testEntity);
            await context.SaveChangesAsync();

            // Act
            await mediatorMock.Object.DispatchDomainEventsAsync(context);

            // Assert
            mediatorMock.Verify(m => m.Publish(It.IsAny<INotification>(), default), Times.Once);
            testEntity.DomainEvents.Should().BeEmpty();
        }

        [Fact]
        public async Task DispatchDomainEventsAsync_ShouldNotDispatchWhenNoDomainEvents()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var mediatorMock = new Mock<IMediator>();
            using var context = new ApplicationDbContext(options);

            var testEntity = new TestEntity();
            context.Add(testEntity);
            await context.SaveChangesAsync();

            // Act
            await mediatorMock.Object.DispatchDomainEventsAsync(context);

            // Assert
            mediatorMock.Verify(m => m.Publish(It.IsAny<INotification>(), default), Times.Never);
        }

        [Fact]
        public async Task DispatchDomainEventsAsync_ShouldClearDomainEventsAfterDispatch()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var mediatorMock = new Mock<IMediator>();
            using var context = new ApplicationDbContext(options);

            var testEntity = new TestEntity();
            testEntity.AddDomainEvent(new TestDomainEvent());
            context.Add(testEntity);
            await context.SaveChangesAsync();

            // Act
            await mediatorMock.Object.DispatchDomainEventsAsync(context);

            // Assert
            testEntity.DomainEvents.Should().BeEmpty();
        }

        public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

            public DbSet<TestEntity> TestEntities { get; set; }
        }

        public class TestEntity : IHasEvents
        {
            public int Id { get; set; }

            public List<INotification> DomainEvents { get; } = new List<INotification>();

            public void AddDomainEvent(INotification eventItem)
            {
                DomainEvents.Add(eventItem);
            }

            public void ClearDomainEvents()
            {
                DomainEvents.Clear();
            }

            public void RemoveDomainEvent(INotification eventItem)
            {
                DomainEvents.Remove(eventItem);
            }
        }

        public class TestDomainEvent : INotification { }
    }
}
