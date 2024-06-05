using System.ComponentModel.DataAnnotations;
using Churchee.Common.ValueTypes;

namespace Churchee.Module.Site.Areas.Site.Models
{
    public class BannerInputModel
    {
        public BannerInputModel()
        {
            Name = string.Empty;
            Url = string.Empty;
            Banner = new Upload(path: "banners");
        }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Url { get; set; }

        [Required]
        [DataType(DataTypes.Upload)]
        public Upload Banner { get; set; }
    }
}
