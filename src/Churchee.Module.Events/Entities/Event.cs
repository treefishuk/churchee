using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Helpers;

namespace Churchee.Module.Events.Entities
{
    public class Event : WebContent
    {
        private Event()
        {

        }

        public Event(Guid applicationTenantId, string sourceName, string sourceId, string title, string description, string content, string locationName, string city, string street, string postCode, string country, decimal? latitude, decimal? longitude, DateTime? start, DateTime? end, string imageUrl)
            : base()
        {
            var eventId = Guid.NewGuid();

            Id = eventId;
            ApplicationTenantId = applicationTenantId;
            CreatedDate = DateTime.Now;
            SourceName = sourceName;
            SourceId = sourceId;
            Title = title;
            Description = description;
            Content = content;
            LocationName = locationName;
            City = city;
            Street = street;
            PostCode = postCode;
            Country = country;
            Latitude = latitude;
            Longitude = longitude;
            Start = start;
            End = end;
            ImageUrl = imageUrl;
            Url = $"/events/{eventId}";
            IsSystem = true;
            PageTypeId = PageTypes.EventDetailPageTypeId;
        }

        public string Content { get; private set; }

        public string LocationName { get; private set; }

        public string City { get; private set; }

        public string Street { get; private set; }

        public string PostCode { get; private set; }

        public string Country { get; private set; }

        public decimal? Latitude { get; private set; }

        public decimal? Longitude { get; private set; }

        public DateTime? Start { get; private set; }

        public DateTime? End { get; private set; }

        public string ImageUrl { get; private set; }

        public void SetImageUrl(string imageUrl)
        {
            ImageUrl = imageUrl;
        }

    }
}
