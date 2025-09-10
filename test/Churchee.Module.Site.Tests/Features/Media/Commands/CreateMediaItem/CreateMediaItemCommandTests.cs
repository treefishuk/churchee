using System;
using Churchee.Module.Site.Features.Media.Commands;
using Xunit;

public class CreateMediaItemCommandTests
{
    [Fact]
    public void CanSetAndGetProperties()
    {
        // Arrange
        var command = new CreateMediaItemCommand
        {
            Name = "TestName",
            FileName = "TestFileName",
            FileExtension = ".jpg",
            LinkUrl = "http://example.com",
            Description = "desc",
            AdditionalContent = "add",
            Base64Content = "base64",
            FolderId = Guid.NewGuid(),
            CssClass = "class",
            SupportedFileTypes = ".jpg,.png",
            Order = 5
        };

        // Assert
        Assert.Equal("TestName", command.Name);
        Assert.Equal("TestFileName", command.FileName);
        Assert.Equal(".jpg", command.FileExtension);
        Assert.Equal("http://example.com", command.LinkUrl);
        Assert.Equal("desc", command.Description);
        Assert.Equal("add", command.AdditionalContent);
        Assert.Equal("base64", command.Base64Content);
        Assert.NotNull(command.FolderId);
        Assert.Equal("class", command.CssClass);
        Assert.Equal(".jpg,.png", command.SupportedFileTypes);
        Assert.Equal(5, command.Order);
    }

    [Fact]
    public void Builder_SetsPropertiesCorrectly()
    {
        // Arrange
        var folderId = Guid.NewGuid();
        var builder = new CreateMediaItemCommand.Builder()
            .SetName("Name")
            .SetFileName("File Name")
            .SetExtension(".png")
            .SetDescription("desc")
            .SetAdditionalContent("add")
            .SetFolderId(folderId)
            .SetBase64Content("base64")
            .SetLinkUrl("url")
            .SetCssClass("css")
            .SetSupportedFileTypes(".png,.jpg")
            .SetOrder(2);

        // Act
        var command = builder.Build();

        // Assert
        Assert.Equal("Name", command.Name);
        Assert.Equal("File-Name", command.FileName); // spaces replaced with dashes
        Assert.Equal(".png", command.FileExtension);
        Assert.Equal("desc", command.Description);
        Assert.Equal("add", command.AdditionalContent);
        Assert.Equal(folderId, command.FolderId);
        Assert.Equal("base64", command.Base64Content);
        Assert.Equal("url", command.LinkUrl);
        Assert.Equal("css", command.CssClass);
        Assert.Equal(".png,.jpg", command.SupportedFileTypes);
        Assert.Equal(2, command.Order);
    }
}
