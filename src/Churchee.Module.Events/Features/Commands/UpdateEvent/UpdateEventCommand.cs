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

        public Guid Id { get; }

        public string Title { get; }

        public string Description { get; }

        public string Content { get; }

        public string ImageFileName { get; }

        public string Base64Image { get; }

        public string LocationName { get; }

        public string City { get; }

        public string Street { get; }

        public string PostCode { get; }

        public string Country { get; }

        public decimal? Latitude { get; }

        public decimal? Longitude { get; }

        public List<EventDateModel> Dates { get; }

    }
}
