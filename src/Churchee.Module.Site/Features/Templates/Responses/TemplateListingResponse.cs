using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Features.Templates.Responses
{
    public class TemplateListingResponse
    {
        [DataType(DataTypes.Hidden)]
        public Guid Id { get; set; }

        public string Location { get; set; }

        [DataType(DataTypes.Hidden)]
        public int HasChildren { get; set; }
    }
}
