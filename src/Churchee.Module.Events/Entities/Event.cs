using Churchee.Module.Site.Entities;

namespace Churchee.Module.Events.Entities
{
    public class Event : WebContent
    {
        public Event()
        {
            Title = string.Empty;
            Description = string.Empty;
            LocationName = string.Empty;
            City = string.Empty;
            Street = string.Empty;
            PostCode = string.Empty;
            Country = string.Empty;
            Content = string.Empty;
            EventDates = [];
        }

        public string Content { get; private set; }

        public string LocationName { get; private set; }

        public string City { get; private set; }

        public string Street { get; private set; }

        public string PostCode { get; private set; }

        public string Country { get; private set; }

        public decimal? Latitude { get; private set; }

        public decimal? Longitude { get; private set; }

        public ICollection<EventDate> EventDates { get; set; }

        public void UpdateLocation(string locationName, string city, string street, string postCode, string country, decimal? latitude, decimal? longitude)
        {
            LocationName = locationName;
            City = city;
            Street = street;
            PostCode = postCode;
            Country = country;
            Latitude = latitude;
            Longitude = longitude;
        }

        public void UpdateInformation(string title, string description, string content)
        {
            Title = title;
            Description = description;
            Content = content;
        }

        public void AddDate(Guid id, DateTime? start, DateTime? end)
        {
            if (start != null)
            {
                EventDates.Add(new EventDate { Id = id, Start = start, End = end, EventId = Id, Event = this, Deleted = false });
            }
        }

        public static void RemoveDate(EventDate date)
        {
            date.Deleted = true;
        }

        public class Builder
        {
            private readonly Event _event = new();

            public string ParentSlug { get; set; }

            public Builder SetApplicationTenantId(Guid applicationTenantId)
            {
                _event.ApplicationTenantId = applicationTenantId;
                return this;
            }

            public Builder SetParentId(Guid? parentId)
            {
                _event.ParentId = parentId;
                return this;
            }

            public Builder SetParentSlug(string parentSlug)
            {
                ParentSlug = parentSlug;
                return this;
            }

            public Builder SetPageTypeId(Guid pageTypeId)
            {
                _event.PageTypeId = pageTypeId;
                return this;
            }

            public Builder SetSourceName(string sourceName)
            {
                _event.SourceName = sourceName;
                return this;
            }

            public Builder SetSourceId(string sourceId)
            {
                _event.SourceId = sourceId;
                return this;
            }

            public Builder SetTitle(string title)
            {
                _event.Title = title;
                return this;
            }

            public Builder SetDescription(string description)
            {
                _event.Description = description;
                return this;
            }

            public Builder SetContent(string content)
            {
                _event.Content = content;
                return this;
            }

            public Builder SetLocationName(string locationName)
            {
                _event.LocationName = locationName;
                return this;
            }

            public Builder SetCity(string city)
            {
                _event.City = city;
                return this;
            }

            public Builder SetStreet(string street)
            {
                _event.Street = street;
                return this;
            }

            public Builder SetPostCode(string postCode)
            {
                _event.PostCode = postCode;
                return this;
            }

            public Builder SetCountry(string country)
            {
                _event.Country = country;
                return this;
            }

            public Builder SetLatitude(decimal? latitude)
            {
                _event.Latitude = latitude;
                return this;
            }

            public Builder SetLongitude(decimal? longitude)
            {
                _event.Longitude = longitude;
                return this;
            }

            public Builder SetDates(DateTime? start, DateTime? end)
            {
                _event.EventDates.Add(new EventDate { Id = Guid.NewGuid(), EventId = _event.Id, Start = start, End = end });
                return this;
            }

            public Builder SetImageUrl(string imageUrl)
            {
                _event.ImageUrl = imageUrl;
                return this;
            }

            public Builder SetPublished(bool published)
            {
                _event.Published = published;
                return this;
            }

            public Event Build()
            {
                _event.Id = Guid.NewGuid();
                _event.Url = $"{ParentSlug}/{_event.Title.ToURL()}";

                return _event;
            }
        }
    }
}
