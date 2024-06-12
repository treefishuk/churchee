using System.ComponentModel.DataAnnotations;
using Churchee.Common.ValueTypes;
using Churchee.Module.Events.Features.Queries;
using Churchee.Module.UI.Models;

namespace Churchee.Module.Events.Areas.Website.Models
{

    public class CreateEventInputModel
    {

        public CreateEventInputModel()
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

        [DataType(DataTypes.DateTime)]
        public DateTime? Start { get; private set; }

        [DataType(DataTypes.DateTime)]
        public DateTime? End { get; private set; }

        [DataType(DataTypes.Html)]
        public string Content { get; private set; }

        [DataType(DataTypes.ImageUpload)]
        public Upload ImageUpload { get; set; }

        public string LocationName { get; private set; }

        public string City { get; private set; }

        public string Street { get; private set; }

        public string PostCode { get; private set; }

        public string Country { get; private set; }


        [DataType(DataTypes.GeoCoordinates)]
        public decimal? Latitude { get; private set; }


        [DataType(DataTypes.GeoCoordinates)]
        public decimal? Longitude { get; private set; }

    }
}
