using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Jotform.Areas.Integrations.Model
{
    public class InputModel
    {
        [DataType(DataType.Password)]
        public string ApiKey { get; set; }
    }
}
