using System.ComponentModel.DataAnnotations;
using Churchee.Module.Site.Features.Pages.Queries;
using Churchee.Module.UI.Models;

namespace Churchee.Module.Site.Areas.Site.Models
{
    public class PageEditModel
    {

        public PageEditModel()
        {
            Title = string.Empty;
            Description = string.Empty;
            Parent = new DropdownInput();
            ContentItems = [];
        }

        [MaxLength(100)]
        [DataType(DataTypes.TextWithSlug)]
        public string Title { get; set; }


        [MaxLength(200)]
        [DataType(DataTypes.MultilineText)]
        public string Description { get; set; }

        public DropdownInput Parent { get; set; }

        public List<GetPageDetailsResponseContentItem> ContentItems { get; set; }

    }
}
