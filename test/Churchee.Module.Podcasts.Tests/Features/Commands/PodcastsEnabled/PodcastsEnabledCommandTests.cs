using Churchee.Module.Podcasts.Features.Commands;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Podcasts.Tests.Features.Commands.PodcastsEnabled
{
    public class PodcastsEnabledCommandTests
    {
        [Fact]
        public void PodcastsEnabledCommand_Correctly_Sets_PageNameForPodcasts()
        {
            //arrange
            string pageNameForPodcasts = "Podcasts";

            //act
            var cut = new PodcastsEnabledCommand(pageNameForPodcasts);

            //assert
            cut.PageNameForPodcasts.Should().Be(pageNameForPodcasts);
        }
    }
}
