using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Churchee.Module.Podcasts.AnchorIntegration.Areas.Integrations.Models
{
    public class InputModel
    {
        public InputModel()
        {
            AnchorRSSFeedUrl = string.Empty;
            NameForContent = string.Empty;
        }

        public InputModel(string anchorRSSFeedUrl, string nameForContent)
        {
            AnchorRSSFeedUrl = anchorRSSFeedUrl ?? string.Empty;
            NameForContent = nameForContent ?? string.Empty;
        }

        [Required]
        [DataType(DataType.Url)]
        public string AnchorRSSFeedUrl { get; set; }

        [Required]
        [DataType(DataTypes.TextWithSlug)]
        public string NameForContent { get; set; }
    }
}
