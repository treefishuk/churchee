using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Podcasts.SpotifyIntegration.Areas.Integrations.Models
{
    public class InputModel
    {
        public InputModel()
        {
            SpotifyRSSFeedUrl = string.Empty;
            NameForContent = string.Empty;
        }

        public InputModel(string spotifyRSSFeedUrl, string nameForContent)
        {
            SpotifyRSSFeedUrl = spotifyRSSFeedUrl ?? string.Empty;
            NameForContent = nameForContent ?? string.Empty;
        }

        [Required]
        [DataType(DataType.Url)]
        public string SpotifyRSSFeedUrl { get; set; }

        [Required]
        [DataType(DataTypes.TextWithSlug)]
        public string NameForContent { get; set; }
    }
}
