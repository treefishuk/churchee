using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Tests.Entities
{
    public class MediaFolderTests
    {
        [Fact]
        public void Constructor_SetsProperties_ForRootFolder()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var name = "Images";
            var supportedFileTypes = ".jpg,.png";

            // Act
            var folder = new MediaFolder(tenantId, name, supportedFileTypes);

            // Assert
            Assert.Equal(tenantId, folder.ApplicationTenantId);
            Assert.Equal(name, folder.Name);
            Assert.Equal($"{name}/", folder.Path);
            Assert.Equal(supportedFileTypes, folder.SupportedFileTypes);
            Assert.Null(folder.ParentId);
            Assert.Null(folder.Parent);
        }

        [Fact]
        public void Constructor_SetsProperties_ForChildFolder()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var parent = new MediaFolder(tenantId, "Parent", ".jpg,.png");
            var childName = "Child";

            // Act
            var child = new MediaFolder(tenantId, childName, parent);

            // Assert
            Assert.Equal(tenantId, child.ApplicationTenantId);
            Assert.Equal(childName, child.Name);
            Assert.Equal($"{parent.Path}{childName}/", child.Path);
            Assert.Equal(parent.SupportedFileTypes, child.SupportedFileTypes);
            Assert.Equal(parent.Id, child.ParentId);
            Assert.Null(child.Parent); // Not set by constructor
        }

        [Fact]
        public void Can_Set_And_Get_Children()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var parent = new MediaFolder(tenantId, "Parent", ".jpg");
            var child = new MediaFolder(tenantId, "Child", parent);
            var children = new List<MediaFolder> { child };

            // Act
            parent.Children = children;

            // Assert
            Assert.NotNull(parent.Children);
            Assert.Single(parent.Children);
            Assert.Equal("Child", ((List<MediaFolder>)parent.Children)[0].Name);
        }

        [Fact]
        public void Can_Set_And_Get_Parent()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var parent = new MediaFolder(tenantId, "Parent", ".jpg");
            var child = new MediaFolder(tenantId, "Child", parent);

            // Act
            child.Parent = parent;

            // Assert
            Assert.Equal(parent, child.Parent);
        }
    }
}