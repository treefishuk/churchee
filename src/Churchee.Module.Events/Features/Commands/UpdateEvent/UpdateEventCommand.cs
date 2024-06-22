using Churchee.Common.ResponseTypes;
using Churchee.Module.Events.Models;
using MediatR;

namespace Churchee.Module.Events.Features.Commands
{
    public class UpdateEventCommand : IRequest<CommandResponse>
    {
        public UpdateEventCommand(Guid id, string title, string description, string content, string imageFileName, string base64Image, string locationName, string city, string street, string postCode, string country, decimal? latitude, decimal? longitude, List<EventDateModel> dates)
        {
            Id = id;
            Title = title;
            Description = description;
            Content = content ?? string.Empty;
            LocationName = locationName;
            City = city ?? string.Empty;
            Street = street ?? string.Empty;
            PostCode = postCode ?? string.Empty;
            Country = country ?? string.Empty;
            Latitude = latitude;
            Longitude = longitude;
            ImageFileName = imageFileName ?? string.Empty;
            Base64Image = base64Image ?? string.Empty;
            Dates = dates;
        }

        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; private set; }

        public string ImageFileName { get; set; }

        public string Base64Image { get; set; }

        public string LocationName { get; private set; }

        public string City { get; private set; }

        public string Street { get; private set; }

        public string PostCode { get; private set; }

        public string Country { get; private set; }

        public decimal? Latitude { get; private set; }

        public decimal? Longitude { get; private set; }

        public List<EventDateModel> Dates { get; private set; }

    }
}
