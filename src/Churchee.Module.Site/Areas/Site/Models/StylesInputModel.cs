using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Areas.Site.Models
{
    public class StylesInputModel
    {
        public StylesInputModel()
        {
            Styles = string.Empty;
        }

        public StylesInputModel(string styles)
        {
            Styles = styles;
        }

        [DataType(DataTypes.CssEditor)]
        public string Styles { get; set; }


    }
}
