using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Reviews.Models
{
    public class CreateReviewInputModel
    {
        public int Rating { get; set; }

        public string ReviewerName { get; set; }


        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }
    }
}
