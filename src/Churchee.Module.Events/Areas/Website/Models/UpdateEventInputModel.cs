using Churchee.Common.ValueTypes;
using Churchee.Module.Events.Features.Queries;
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

        public UpdateEventInputModel(GetDetailByIdResponse responseObject)
        {
            Title = responseObject.Title;
            Description = responseObject.Description;
            Content = responseObject.Content;
            LocationName = responseObject.LocationName;
            City = responseObject.City;
            Street = responseObject.Street;
            PostCode = responseObject.PostCode;
            Country = responseObject.Country;
            Latitude = responseObject.Latitude;
            Longitude = responseObject.Longitude;
            ImageUpload = new Upload();
            Dates = responseObject.Dates;
        }

        [Required]
        [DataType(DataTypes.TextWithSlug)]
        public string Title { get; set; }

        [Required]
        [MaxLength(200)]
        [DataType(DataTypes.MultilineText)]
        public string Description { get; set; }

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

        public List<EventDateModel> Dates { get; private set; }

    }
}
