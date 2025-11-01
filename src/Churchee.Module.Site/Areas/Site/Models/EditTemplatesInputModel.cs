using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Areas.Site.Models
{
    public class EditTemplateInputModel
    {
        public EditTemplateInputModel()
        {
            Content = string.Empty;
        }

        public EditTemplateInputModel(Guid id, string content)
        {
            Id = id;
            Content = content;
        }

        [Required]
        [DataType(DataTypes.Hidden)]
        [NotEmptyGuid]
        public Guid Id { get; set; }

        [Required]
        [DataType(DataTypes.RazorEditor)]
        public string Content { get; set; }

    }
}
