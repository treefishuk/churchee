using Churchee.Common.ValueTypes;
using Churchee.Module.Events.Models;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Events.Areas.Website.Models
{

    public class UpdateEventInputModel
    {
        public UpdateEventInputModel()
        {
            Title = string.Empty;
            Description = string.Empty;
            ImageUpload = new Upload();
            LocationName = string.Empty;
            City = string.Empty;
            Street = string.Empty;
            PostCode = string.Empty;
            Country = string.Empty;
            Content = string.Empty;
        }

        [Required]
        [DataType(DataTypes.TextWithSlug)]
        public string Title { get; set; }

        [Required]
        [MaxLength(200)]
        [DataType(DataTypes.MultilineText)]
        public string Description { get; set; }

        [DataType(DataTypes.Html)]
        public string Content { get; set; }

        [DataType(DataTypes.ImageUpload)]
        public Upload ImageUpload { get; set; }

        public string LocationName { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string PostCode { get; set; }

        public string Country { get; set; }

        [DataType(DataTypes.GeoCoordinates)]
        public decimal? Latitude { get; set; }

        [DataType(DataTypes.GeoCoordinates)]
        public decimal? Longitude { get; set; }

        public List<EventDateModel> Dates { get; set; }

    }
}
