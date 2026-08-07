using Churchee.Module.ChurchSuite.Events.Specifications;
using Churchee.Module.Events.Entities;

namespace Churchee.Module.Facebook.Events.Tests.Specifications
{
    public class GetEventByChurchSuiteSequenceSpecificationTests
    {
        [Fact]
        public void GetEventByFacebookIdSpecification_Should_Return_SingleResult()
        {
            Guid tenantId = Guid.NewGuid();

            var events = new List<Event>
            {
                new Event.Builder()
                    .SetApplicationTenantId(tenantId)
                    .SetSourceId("12345")
                    .SetTitle("Title 1")
                    .Build(),
                new Event.Builder()
                    .SetApplicationTenantId(tenantId)
                    .SetSourceId("234532")
                    .SetTitle("Title 1")
                    .Build(),
                new Event.Builder()
                    .SetApplicationTenantId(tenantId)
                    .SetSourceId("112442")
                    .SetTitle("Title 1")
                    .Build(),
            };

            var spec = new GetEventByChurchSuiteSequenceSpecification("12345", tenantId);

            // Act
            var result = spec.Evaluate(events);

            // Assert
            Assert.Single(result);
        }
    }
}
