using Churchee.Module.Events.Models;

namespace Churchee.Module.Events.Features.Queries
{
    public class GetDetailByIdResponse
    {
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

        public List<EventDateModel> Dates { get; private set; }


        public class Builder
        {
            private readonly GetDetailByIdResponse _response = new();

            public Builder SetTitle(string title)
            {
                _response.Title = title;
                return this;
            }

            public Builder SetDescription(string description)
            {
                _response.Description = description;
                return this;
            }

            public Builder SetContent(string content)
            {
                _response.Content = content;
                return this;
            }

            public Builder SetImageUrl(string imageUrl)
            {
                _response.ImageUrl = imageUrl;
                return this;
            }

            public Builder SetLocationName(string locationName)
            {
                _response.LocationName = locationName;
                return this;
            }

            public Builder SetCity(string city)
            {
                _response.City = city;
                return this;
            }

            public Builder SetStreet(string street)
            {
                _response.Street = street;
                return this;
            }

            public Builder SetPostCode(string postCode)
            {
                _response.PostCode = postCode;
                return this;
            }

            public Builder SetCountry(string country)
            {
                _response.Country = country;
                return this;
            }

            public Builder SetLatitude(decimal? latitude)
            {
                _response.Latitude = latitude;
                return this;
            }

            public Builder SetLongitude(decimal? longitude)
            {
                _response.Longitude = longitude;
                return this;
            }

            public Builder SetDates(List<EventDateModel> dates)
            {
                _response.Dates = dates;
                return this;
            }

            public GetDetailByIdResponse Build()
            {
                return _response;
            }

        }
    }
}
