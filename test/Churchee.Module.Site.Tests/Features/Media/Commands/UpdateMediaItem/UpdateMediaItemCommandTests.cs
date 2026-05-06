using Churchee.Module.Site.Features.Media.Commands;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Site.Tests.Features.Media.Commands.UpdateMediaItem
{
    public class UpdateMediaItemCommandTests
    {
        [Fact]
        public void UpdateMediaItemCommand_Builder_Set_Simple_FileName_Works()
        {
            // Arrange
            var command = new UpdateMediaItemCommand.Builder()
                .SetId(Guid.NewGuid())
                .SetFileName("fileName")
                .SetName("Name")
                .SetDescription("Desc")
                .SetOrder(0)
                .Build();

            // Act & Assert
            command.FileName.Should().Be("fileName");
        }

        [Fact]
        public void UpdateMediaItemCommand_Builder_Set_Complex_FileName_Works()
        {
            // Arrange
            var command = new UpdateMediaItemCommand.Builder()
                .SetId(Guid.NewGuid())
                .SetFileName("I am a new file")
                .SetName("Name")
                .SetDescription("Desc")
                .SetOrder(0)
                .Build();

            // Act & Assert
            command.FileName.Should().Be("I-am-a-new-file");
        }
    }
}
