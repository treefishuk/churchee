using Churchee.Module.Site.Helpers;
using Churchee.Module.UI.Models;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Areas.Site.Models
{
    public class PageTypeContentItemModel
    {

        public PageTypeContentItemModel()
        {
            Name = "New Content Type";
            Order = 0;
            Required = false;
            Type = new DropdownInput()
            {
                Title = EditorType.SimpleText,
                Value = EditorType.SimpleText,
                Data = EditorType.All().Select(s => new DropdownInput { Title = s, Value = s }).ToList()
            };
            Id = Guid.NewGuid();
        }

        [DataType(DataTypes.Hidden)]
        [NotEmptyGuid]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DropdownInput Type { get; set; }

        public bool Required { get; set; }

        public int Order { get; set; }

    }
}
