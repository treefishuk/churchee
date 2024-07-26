using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Areas.Site.Models
{
    public class CreateTemplatesInputModel
    {
        public CreateTemplatesInputModel()
        {
            Path = string.Empty;
            Content = string.Empty;
        }

        [Required]
        public string Path { get; set; }

        [Required]
        [DataType(DataTypes.RazorEditor)]
        public string Content { get; set; }

    }
}
