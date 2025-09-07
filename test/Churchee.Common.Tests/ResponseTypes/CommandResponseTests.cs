using Churchee.Common.ResponseTypes;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Common.Tests.ResponseTypes
{
    public class CommandResponseTests
    {
        [Fact]
        public void CommandResponse_NoErrors_StateIsValid()
        {
            var cut = new CommandResponse();

            cut.IsSuccess.Should().BeTrue();
            cut.Errors.Should().BeEmpty();
        }

        [Fact]
        public void CommandResponse_WIthErrors_StateIsValid()
        {
            var cut = new CommandResponse();

            cut.AddError("Test", "Thing");

            cut.IsSuccess.Should().BeFalse();
            cut.Errors.Count.Should().Be(1);
        }

    }
}
