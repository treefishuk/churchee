using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Areas.Site.Models
{
    public class TemplatesInputModel
    {
        public TemplatesInputModel()
        {
            Content = string.Empty;
        }

        public TemplatesInputModel(Guid id, string content)
        {
            Id = id;
            Content = content;
        }

        [Required]
        [DataType(DataTypes.Hidden)]
        public Guid Id { get; set; }

        [Required]
        [DataType(DataTypes.RazorEditor)]
        public string Content { get; set; }

    }
}
