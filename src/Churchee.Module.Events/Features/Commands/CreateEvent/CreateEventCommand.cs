using Churchee.Common.ResponseTypes;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? Start { get; private set; }

        public DateTime? End { get; private set; }

        public string Content { get; private set; }

        public string ImageFileName { get; set; }

        public string Base64Image{ get; set; }

        public string LocationName { get; private set; }

        public string City { get; private set; }

        public string Street { get; private set; }

        public string PostCode { get; private set; }

        public string Country { get; private set; }

        public decimal? Latitude { get; private set; }

        public decimal? Longitude { get; private set; }


    }
}
