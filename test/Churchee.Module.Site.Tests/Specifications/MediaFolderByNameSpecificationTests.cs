using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Tests.Specifications
{
    public class MediaFolderByNameSpecificationTests
    {
        [Fact]
        public void Constructor_Sets_Query()
        {
            string folder = "images";
            var tenant = Guid.NewGuid();

            var spec = new MediaFolderByNameSpecification(folder, tenant);

            Assert.NotNull(spec);
        }

        [Fact]
        public void Evaluates_Filter_By_Name_And_Tenant()
        {
            string folderName = "images";
            var tenant = Guid.NewGuid();

            var match = new MediaFolder(tenant, folderName, "jpg,png");
            var other = new MediaFolder(tenant, "other", "jpg");
            var otherTenant = new MediaFolder(Guid.NewGuid(), folderName, "jpg");

            var list = new[] { match, other, otherTenant };

            var spec = new MediaFolderByNameSpecification(folderName, tenant);

            var results = spec.Evaluate(list).ToList();

            _ = Assert.Single(results);
            Assert.Contains(results, r => r.Id == match.Id);
        }
    }
}
