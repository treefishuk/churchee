using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Events.Features.Commands
{
    public class CreateEventCommand : IRequest<CommandResponse>
    {
        public CreateEventCommand(string title, string description, DateTime? start, DateTime? end, string content, string imageFileName, string base64Image, string locationName, string city, string street, string postCode, string country, decimal? latitude, decimal? longitude)
        {
            Title = title;
            Description = description;
            Start = start;
            End = end;
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
        }

        public string Title { get; }

        public string Description { get; }

        public DateTime? Start { get; }

        public DateTime? End { get; }

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


    }
}
