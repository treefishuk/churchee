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
        [Display(Name = "Spotify RSS Feed Url", Description = "Found in: Settings → Availability → RSS Distribution", GroupName = "Feed")]
        public string SpotifyRSSFeedUrl { get; set; }

        [Required]
        [DataType(DataTypes.TextWithSlug)]
        [Display(Description = "Will be used in Url for the pulled content. ex. \"Listen\"", GroupName = "Destination")]
        public string NameForContent { get; set; }
    }
}
