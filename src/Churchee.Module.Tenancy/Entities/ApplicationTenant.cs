using Churchee.Common.Abstractions.Entities;
using MediatR;

namespace Churchee.Module.Tenancy.Entities
{
    public class ApplicationTenant : IEntity, IHasEvents
    {
        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        private List<INotification> _domainEvents;

        public List<INotification> DomainEvents => _domainEvents;

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        private ApplicationTenant()
        {
        }

        public ApplicationTenant(Guid tenantId, string name, int charityNumber)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Id = tenantId;
            Name = name;
            DevName = name.ToCamelCase();
            CharityNumber = charityNumber;
            Hosts = new List<ApplicationHost>();
            Features = new List<ApplicationFeature>();
        }

        public ApplicationTenant(string name, int charityNumber) : base()
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            DevName = name.ToCamelCase();
            CharityNumber = charityNumber;
            Hosts = new List<ApplicationHost>();
            Features = new List<ApplicationFeature>();
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string DevName { get; private set; }

        public int CharityNumber { get; private set; }

        public ICollection<ApplicationHost> Hosts { get; private set; }

        public ICollection<ApplicationFeature> Features { get; private set; }
        public bool Deleted { get; set; }

        public int SetName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;

            return 1;

        }

        public int AddHost(string hostname)
        {
            if (string.IsNullOrEmpty(hostname))
            {
                throw new ArgumentNullException(nameof(hostname));
            }

            Hosts.Add(new ApplicationHost(hostname, Id));

            return 1;
        }

        public int RemoveHost(Guid hostId)
        {
            var host = Hosts
                .Where(x => x.Id == hostId)
                .FirstOrDefault();

            Hosts.Remove(host);

            return 1;
        }

        public int AddFeature(string featureName)
        {
            if (string.IsNullOrEmpty(featureName))
            {
                throw new ArgumentNullException(nameof(featureName));
            }

            Features.Add(new ApplicationFeature(featureName, Id));

            return 1;
        }

        public int RemoveFeature(Guid featureId)
        {
            var feature = Features
                .Where(x => x.Id == featureId)
                .FirstOrDefault();

            Features.Remove(feature);

            return 1;
        }

    }
}
