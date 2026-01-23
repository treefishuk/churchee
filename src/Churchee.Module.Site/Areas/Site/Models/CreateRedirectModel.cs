using Churchee.Module.UI.Models;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Areas.Site.Models
{
    public class CreateRedirectModel
    {
        public CreateRedirectModel()
        {
            Path = string.Empty;
            Page = new DropdownInput()
            {
                Title = "Select a Page",
                Value = string.Empty,
                Data = []
            };
        }

        [Required]
        public string Path { get; set; }

        [Required]
        public DropdownInput Page { get; set; }

    }
}
