using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.YouTube.Areas.Integrations.Models
{
    public class InputModel
    {
        public InputModel()
        {
            ChannelIdentifier = string.Empty;
            ApiKey = string.Empty;
            NameForContent = string.Empty;
        }

        [Required]
        [DataType(DataTypes.Password)]
        [RegularExpression(@"^AIza[0-9A-Za-z_-]{35}$", ErrorMessage = "Invalid YouTube/Google API key format.")]
        [Display(Name = "YouTube API Key", Description = "Used to fetch videos", GroupName = "YouTube API Settings")]
        public string ApiKey { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Channel Id/ Handle", GroupName = "Content Source", Description = "@channel or channel ID")]
        [RegularExpression(@"^(?:UC[A-Za-z0-9_-]{22}|@[\p{L}\p{N}_\.-]{2,29})$", ErrorMessage = "Enter a valid channel handle (e.g. @handle) or channel ID (starts with UC...).")]
        public string ChannelIdentifier { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Description = "Determines the Url for the videos", GroupName = "Display Settings")]
        public string NameForContent { get; set; }

    }
}
