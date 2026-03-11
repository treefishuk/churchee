using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Reviews.Features.Queries
{
    public class GetListingQueryResponseItem
    {

        [DataType(DataTypes.Hidden)]
        public Guid Id { get; set; }

        [DataType(DataTypes.ImageUrl)]
        public string ReviewerImageUrl { get; set; }

        public string Reviewer { get; set; }

        [DataType(DataTypes.Rating)]
        public int Rating { get; set; }

        [Display(Name = "Created")]
        [DataType(DataTypes.Date)]
        public DateTime CreatedDate { get; set; }



    }
}
