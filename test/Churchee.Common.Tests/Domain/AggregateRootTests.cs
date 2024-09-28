﻿using Churchee.Common.Data;
using FluentAssertions;
using MediatR;
using Moq;

namespace Churchee.Common.Tests.Domain
{
    public class AggregateRootTests
    {

        [Fact]
        public void AggregateRoot_WithApplicationIdConstructor_SetsPropertiesCorrectly()
        {
            //arrange
            var applicationTenantId = Guid.NewGuid();

            //act
            var cut = new Mock<AggregateRoot>(applicationTenantId).Object;

            //assert
            cut.ApplicationTenantId.Should().Be(applicationTenantId);
            cut.Id.Should().NotBeEmpty();
            cut.CreatedDate.Should().NotBeNull();
            cut.ModifiedDate.Should().NotBeNull();
            cut.CreatedById.Should().Be(Guid.Empty);
            cut.ModifiedById.Should().Be(Guid.Empty);
            cut.Deleted.Should().Be(false);

        }

        [Fact]
        public void AggregateRoot_WithIdAndApplicationIdConstructor_SetsPropertiesCorrectly()
        {
            //arrange
            var applicationTenantId = Guid.NewGuid();
            var id = Guid.NewGuid();

            //act
            var cut = new Mock<AggregateRoot>(id, applicationTenantId).Object;

            //assert
            cut.ApplicationTenantId.Should().Be(applicationTenantId);
            cut.Id.Should().Be(id);
            cut.CreatedDate.Should().NotBeNull();
            cut.ModifiedDate.Should().NotBeNull();
            cut.CreatedById.Should().Be(Guid.Empty);
            cut.ModifiedById.Should().Be(Guid.Empty);
            cut.Deleted.Should().Be(false);
        }

        [Fact]
        public void AggregateRoot_DomainEventsShouldReturnNullOnInit()
        {
            //act
            var cut = new Mock<AggregateRoot>().Object;

            //assert
            cut.DomainEvents.Should().BeNull();
        }

        [Fact]
        public void AggregateRoot_AddDomainEvent_DomainEventsShouldReturnNotification()
        {
            //arrange
            var cut = new Mock<AggregateRoot>().Object;
            var notification = new Mock<INotification>().Object;

            //act
            cut.AddDomainEvent(notification);

            //assert
            cut.DomainEvents.Should().NotBeNull();
            cut.DomainEvents.Count().Should().Be(1);
        }

        [Fact]
        public void AggregateRoot_RemoveDomainEvent_DomainEventsShouldNotReturnEvent()
        {
            //arrange
            var cut = new Mock<AggregateRoot>().Object;
            var notification = new Mock<INotification>().Object;
            cut.AddDomainEvent(notification);

            //act
            cut.RemoveDomainEvent(notification);

            //assert
            cut.DomainEvents.Should().NotBeNull();
            cut.DomainEvents.Count().Should().Be(0);
        }

        [Fact]
        public void AggregateRootClearDomainEvents_DomainEventsShouldNotReturnEvent()
        {
            //arrange
            var cut = new Mock<AggregateRoot>().Object;
            var notification = new Mock<INotification>().Object;
            cut.AddDomainEvent(notification);

            //act
            cut.ClearDomainEvents();

            //assert
            cut.DomainEvents.Should().NotBeNull();
            cut.DomainEvents.Count().Should().Be(0);
        }
    }
}
