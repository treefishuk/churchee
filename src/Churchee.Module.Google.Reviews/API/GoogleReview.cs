namespace Churchee.Module.Google.Reviews.API
{
    public class GoogleReview
    {
        public string Name { get; set; } = null!;
        public string Comment { get; set; } = null!;
        public int StarRating { get; set; }
        public GoogleReviewer Reviewer { get; set; } = null!;
        public DateTime CreateTime { get; set; }
    }
}
