using Churchee.Common.ResponseTypes;
using Churchee.Module.Events.Models;
using MediatR;

namespace Churchee.Module.Events.Features.Commands
{
    public class UpdateEventCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; private set; }

        public string Title { get; private set; }

        public string Description { get; private set; }

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

        public List<EventDateModel> Dates { get; private set; }

        public class Builder
        {
            private readonly UpdateEventCommand _command = new();

            public Builder SetId(Guid id)
            {
                _command.Id = id;
                return this;
            }

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

            public Builder SetGeoCordinates(decimal? latitude, decimal? longitude)
            {
                _command.Latitude = latitude;
                _command.Longitude = longitude;

                return this;
            }

            public Builder SetDates(List<EventDateModel> dates)
            {
                _command.Dates = dates;
                return this;
            }

            public UpdateEventCommand Build()
            {
                Validate();
                return _command;
            }

            private void Validate()
            {
                if (_command.Id == Guid.Empty)
                {
                    throw new InvalidOperationException("Id must be provided.");
                }

                if (string.IsNullOrWhiteSpace(_command.Title))
                {
                    throw new InvalidOperationException("Title must be provided.");
                }

                if (string.IsNullOrWhiteSpace(_command.Description))
                {
                    throw new InvalidOperationException("Description must be provided.");
                }

                foreach (var date in _command.Dates)
                {
                    if (date.Start > date.End)
                    {
                        throw new InvalidOperationException("Start date must be before end date for each event date.");
                    }
                }
            }
        }
    }
}
