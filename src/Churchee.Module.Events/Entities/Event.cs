using Churchee.Module.Site.Entities;

namespace Churchee.Module.Events.Entities
{
    public class Event : WebContent
    {
        private Event()
        {
            Title = string.Empty;
            Description = string.Empty;
            ImageUrl = string.Empty;
            LocationName = string.Empty;
            City = string.Empty;
            Street = string.Empty;
            PostCode = string.Empty;
            Country = string.Empty;
            Content = string.Empty;
            EventDates = new List<EventDate>();
        }

        public Event(Guid applicationTenantId, Guid pageTypeId, string sourceName, string sourceId, string title, string description, string content, string locationName, string city, string street, string postCode, string country, decimal? latitude, decimal? longitude, DateTime? start, DateTime? end, string imageUrl)
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
            ImageUrl = imageUrl;
            Url = $"/events/{eventId}";
            IsSystem = true;
            PageTypeId = pageTypeId;

            EventDates = new List<EventDate>
            {
                new EventDate { Id = Guid.NewGuid(), EventId = eventId, Start = start, End = end }
            };
        }

        public string Content { get; private set; }

        public string LocationName { get; private set; }

        public string City { get; private set; }

        public string Street { get; private set; }

        public string PostCode { get; private set; }

        public string Country { get; private set; }

        public decimal? Latitude { get; private set; }

        public decimal? Longitude { get; private set; }

        public string ImageUrl { get; private set; }

        public void SetImageUrl(string imageUrl)
        {
            ImageUrl = imageUrl;
        }

        public ICollection<EventDate> EventDates { get; set; }

        public void UpdateInfo(string title, string description, string content, string locationName, string city, string street, string postCode, string country, decimal? latitude, decimal? longitude)
        {
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
        }

        public void AddDate(Guid id, DateTime? start, DateTime? end)
        {
            if (start != null)
            {
                EventDates.Add(new EventDate { Id = id, Start = start, End = end, EventId = Id, Event = this, Deleted = false });
            }
        }

        public void RemoveDate(EventDate date)
        {
            date.Deleted = true;
        }

    }
}
