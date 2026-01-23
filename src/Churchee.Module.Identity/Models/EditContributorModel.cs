using Churchee.Common.ValueTypes;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Identity.Models
{
    public class EditContributorModel
    {

        public EditContributorModel(MultiSelect roles)
        {
            Roles = roles;
        }

        [DataType(DataTypes.CheckboxList)]
        public MultiSelect Roles { get; set; }

    }
}
