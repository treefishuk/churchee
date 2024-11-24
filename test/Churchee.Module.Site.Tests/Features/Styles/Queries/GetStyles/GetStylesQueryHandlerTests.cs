using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Styles.Queries;
using FluentAssertions;
using Moq;

namespace Churchee.Module.Site.Tests.Features.Styles.Queries.GetStyles
{
    public class GetStylesQueryHandlerTests
    {
        private readonly Mock<IDataStore> _dataStoreMock;
        private readonly Mock<IRepository<Css>> _repositoryMock;
        private readonly GetStylesQueryHandler _handler;

        public GetStylesQueryHandlerTests()
        {
            _dataStoreMock = new Mock<IDataStore>();
            _repositoryMock = new Mock<IRepository<Css>>();
            _dataStoreMock.Setup(ds => ds.GetRepository<Css>()).Returns(_repositoryMock.Object);
            _handler = new GetStylesQueryHandler(_dataStoreMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnString()
        {
            // Arrange
            const string styles = "body{ background: #333; }";

            var cssStyles = new Css(Guid.Parse("298dccb4-01f8-448b-826b-3e9696240409"));

            cssStyles.SetStyles(styles);

            _repositoryMock
                 .Setup(s => s.FirstOrDefaultAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync(cssStyles);

            // Act
            var result = await _handler.Handle(new GetStylesQuery(), CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().Be(styles);
        }


    }
}
