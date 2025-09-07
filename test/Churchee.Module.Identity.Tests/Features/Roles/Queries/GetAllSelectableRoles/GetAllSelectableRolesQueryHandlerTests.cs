using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Common.ValueTypes;
using Churchee.Module.Events.Specifications;
using Churchee.Module.Identity.Entities;
using Churchee.Module.Identity.Features.Roles.Queries;
using Churchee.Test.Helpers.Validation;
using Moq;
using System.Linq.Expressions;



namespace Churchee.Module.Identity.Tests.Features.Roles.Queries.GetAllSelectableRoles
{
    public class GetAllSelectableRolesQueryHandlerTests
    {
        private readonly Mock<IDataStore> _dataStoreMock;
        private readonly Mock<IRepository<ApplicationRole>> _repositoryMock;
        private readonly GetAllSelectableRolesQueryHandler _handler;

        public GetAllSelectableRolesQueryHandlerTests()
        {
            _dataStoreMock = new Mock<IDataStore>();
            _repositoryMock = new Mock<IRepository<ApplicationRole>>();
            _dataStoreMock.Setup(ds => ds.GetRepository<ApplicationRole>()).Returns(_repositoryMock.Object);
            _handler = new GetAllSelectableRolesQueryHandler(_dataStoreMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSelectableRoles()
        {
            // Arrange
            var roles = new List<MultiSelectItem>
            {
                    new MultiSelectItem(Guid.Parse("298dccb4-01f8-448b-826b-3e9696240409"), "Admin"),
                    new MultiSelectItem(Guid.Parse("2c198722-01ea-480f-b69a-f96c4b81359f"), "User")
            };


            _repositoryMock.Setup(s => s.GetListAsync(It.IsAny<SelectableRolesSpecification>(), It.IsAny<Expression<Func<ApplicationRole, MultiSelectItem>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(roles);


            var query = new GetAllSelectableRolesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.Should().ContainSingle(r => r.Value == Guid.Parse("298dccb4-01f8-448b-826b-3e9696240409") && r.Text == "Admin");
            result.Should().ContainSingle(r => r.Value == Guid.Parse("2c198722-01ea-480f-b69a-f96c4b81359f") && r.Text == "User");
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoSelectableRoles()
        {
            // Arrange
            var roles = new List<MultiSelectItem>();

            _repositoryMock.Setup(s => s.GetListAsync(It.IsAny<SelectableRolesSpecification>(), It.IsAny<Expression<Func<ApplicationRole, MultiSelectItem>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(roles);

            var query = new GetAllSelectableRolesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

    }

}


