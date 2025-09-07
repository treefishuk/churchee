using Bogus;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Common.ValueTypes;
using Churchee.Module.Events.Specifications;
using Churchee.Module.Identity.Entities;
using Churchee.Module.Identity.Features.Roles.Queries;
using Churchee.Module.Identity.Managers;
using Churchee.Module.Identity.Tests.Helpers;
using Churchee.Test.Helpers.Validation;
using Moq;
using System.Linq.Expressions;

namespace Churchee.Module.Identity.Tests.Features.Roles.Queries
{

    public class GetAllSelectableRolesForUserQueryHandlerTests
    {
        private readonly Mock<IDataStore> _dataStoreMock;
        private readonly Mock<IRepository<ApplicationRole>> _repositoryMock;
        private readonly Mock<ChurcheeUserManager> _userManagerMock;
        private readonly GetAllSelectableRolesForUserQueryHandler _handler;
        private readonly Faker _faker = new("en");
        public GetAllSelectableRolesForUserQueryHandlerTests()
        {
            _dataStoreMock = new Mock<IDataStore>();
            _repositoryMock = new Mock<IRepository<ApplicationRole>>();
            _userManagerMock = ChurcheeManagerHelpers.CreateMockChurcheeUserManager();
            _dataStoreMock.Setup(ds => ds.GetRepository<ApplicationRole>()).Returns(_repositoryMock.Object);
            _handler = new GetAllSelectableRolesForUserQueryHandler(_dataStoreMock.Object, _userManagerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSelectableRolesForUser()
        {
            // Arrange
            var user = new ApplicationUser(Guid.NewGuid(), _faker.Internet.UserName(), _faker.Internet.Email());

            var userRoles = new List<string> { "Admin" };

            var roles = new List<MultiSelectItem>
            {
                    new(Guid.Parse("298dccb4-01f8-448b-826b-3e9696240409"), "Admin", true),
                    new(Guid.Parse("2c198722-01ea-480f-b69a-f96c4b81359f"), "User", true)
            };

            _userManagerMock.Setup(um => um.FindByIdAsync(user.Id.ToString())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(userRoles);
            _repositoryMock.Setup(s => s.GetListAsync(It.IsAny<SelectableRolesSpecification>(), It.IsAny<Expression<Func<ApplicationRole, MultiSelectItem>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(roles);

            var query = new GetAllSelectableRolesForUserQuery(user.Id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.Should().ContainSingle(r => r.Value == roles[0].Value && r.Text == roles[0].Text && r.Selected);
            result.Should().ContainSingle(r => r.Value == roles[1].Value && r.Text == roles[1].Text && r.Selected);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoSelectableRoles()
        {
            // Arrange
            var user = new ApplicationUser(Guid.NewGuid(), _faker.Internet.UserName(), _faker.Internet.Email());

            var userRoles = new List<string> { "Admin" };

            var roles = new List<MultiSelectItem>();

            _userManagerMock.Setup(um => um.FindByIdAsync(user.Id.ToString())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(userRoles);
            _repositoryMock.Setup(s => s.GetListAsync(It.IsAny<SelectableRolesSpecification>(), It.IsAny<Expression<Func<ApplicationRole, MultiSelectItem>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(roles);

            var query = new GetAllSelectableRolesForUserQuery(user.Id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync((ApplicationUser?)null);

            var query = new GetAllSelectableRolesForUserQuery(userId);

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("User not found");
        }
    }

}
