using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Events.Features.Commands
{
    public class CreateEventCommand : IRequest<CommandResponse>
    {
        public string Title { get; private set; }

        public string Description { get; private set; }

        public DateTime? Start { get; private set; }

        public DateTime? End { get; private set; }

        public string Content { get; private set; }

        public string ImageFileName { get; private set; }

        public string Base64Image { get; private set; }

        public string LocationName { get; private set; }

        public string City { get; private set; }

        public string Street { get; private set; }

        public string PostCode { get; private set; }

        public string Country { get; private set; }

        public decimal? Latitude { get; private set; }

        public decimal? Longitude { get; private set; }

        public class Builder
        {
            private readonly CreateEventCommand _command = new();

            public Builder SetTitle(string title)
            {
                _command.Title = title;
                return this;
            }

            public Builder SetDescription(string description)
            {
                _command.Description = description;
                return this;
            }

            public Builder SetStart(DateTime? start)
            {
                _command.Start = start;
                return this;
            }

            public Builder SetEnd(DateTime? end)
            {
                _command.End = end;
                return this;
            }

            public Builder SetContent(string content)
            {
                _command.Content = content ?? string.Empty;
                return this;
            }

            public Builder SetImageFileName(string imageFileName)
            {
                _command.ImageFileName = imageFileName ?? string.Empty;
                return this;
            }

            public Builder SetBase64Image(string base64Image)
            {
                _command.Base64Image = base64Image ?? string.Empty;
                return this;
            }

            public Builder SetLocationName(string locationName)
            {
                _command.LocationName = locationName;
                return this;
            }

            public Builder SetCity(string city)
            {
                _command.City = city ?? string.Empty;
                return this;
            }

            public Builder SetStreet(string street)
            {
                _command.Street = street ?? string.Empty;
                return this;
            }

            public Builder SetPostCode(string postCode)
            {
                _command.PostCode = postCode ?? string.Empty;
                return this;
            }

            public Builder SetCountry(string country)
            {
                _command.Country = country ?? string.Empty;
                return this;
            }

            public Builder SetLatitude(decimal? latitude)
            {
                _command.Latitude = latitude;
                return this;
            }

            public Builder SetLongitude(decimal? longitude)
            {
                _command.Longitude = longitude;
                return this;
            }

            public CreateEventCommand Build()
            {
                Validate();
                return _command;
            }

            private void Validate()
            {
                if (string.IsNullOrWhiteSpace(_command.Title))
                {
                    throw new InvalidOperationException("Title must be provided.");
                }

                if (string.IsNullOrWhiteSpace(_command.Description))
                {
                    throw new InvalidOperationException("Description must be provided.");
                }

                if (!_command.Start.HasValue)
                {
                    throw new InvalidOperationException("Start date must be provided.");
                }

                if (_command.Start > _command.End)
                {
                    throw new InvalidOperationException("Start date must be before end date.");
                }

                if (string.IsNullOrWhiteSpace(_command.LocationName))
                {
                    throw new InvalidOperationException("Location name must be provided.");
                }
            }
        }
    }
}
