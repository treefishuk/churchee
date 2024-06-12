using Churchee.Common.ValueTypes;

namespace Churchee.Module.Events.Features.Queries
{
    public class GetDetailByIdResponse
    {
        public GetDetailByIdResponse(string title, string description, string content, string imageUrl, string locationName, string city, string street, string postCode, string country, decimal? latitude, decimal? longitude, List<(DateTime? start, DateTime? end)> dates)
        {
            Title = title;
            Description = description;
            Content = content;
            ImageUrl = imageUrl;
            LocationName = locationName;
            City = city;
            Street = street;
            PostCode = postCode;
            Country = country;
            Latitude = latitude;
            Longitude = longitude;
            Dates = dates;
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; private set; }

        public string ImageUrl { get; set; }

        public string LocationName { get; private set; }

        public string City { get; private set; }

        public string Street { get; private set; }

        public string PostCode { get; private set; }

        public string Country { get; private set; }

        public decimal? Latitude { get; private set; }

        public decimal? Longitude { get; private set; }

        public List<(DateTime? start, DateTime? end)> Dates { get; private set; }

    }
}
