using Churchee.Common.Abstractions.Entities;
using FluentAssertions;

namespace Churchee.Common.Tests.Extensions
{
    public class ObjectExtensionsTests
    {
        [Fact]
        public void NotificationService_ImplementsInterface_ReturnsTrueWhenTrue()
        {
            //arrange
            var cut = new TrueTestClass();

            //act
            var result = cut.ImplementsInterface<ITrackable>();

            //assert
            result.Should().BeTrue();
        }

        [Fact]
        public void NotificationService_ImplementsInterface_ReturnsFalseWhenFalse()
        {
            //arrange
            var cut = new FalseTestClass();

            //act
            var result = cut.ImplementsInterface<ITrackable>();

            //assert
            result.Should().BeFalse();
        }

        private class TrueTestClass : ITrackable
        {
            public Guid? CreatedById => throw new NotImplementedException();

            public string CreatedByUser => throw new NotImplementedException();

            public DateTime? CreatedDate => throw new NotImplementedException();

            public Guid? ModifiedById => throw new NotImplementedException();

            public string ModifiedByName => throw new NotImplementedException();

            public DateTime? ModifiedDate => throw new NotImplementedException();
        }

        private class FalseTestClass
        {

        }
    }
}
