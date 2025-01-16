using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Helpers;
using FluentAssertions;
using Moq;
using System.Reflection;

namespace Churchee.Common.Tests.Helpers
{
    public class AssemblyResolutionTests
    {
        [Fact]
        public void GetModuleAssemblies_ShouldReturnFilteredAssemblies()
        {
            // Arrange
            var mockAssembly1 = new Mock<Assembly>();
            mockAssembly1.Setup(a => a.FullName).Returns("Churchee.Module1");
            mockAssembly1.Setup(a => a.GetTypes()).Returns([typeof(TestModule1)]);

            var mockAssembly2 = new Mock<Assembly>();
            mockAssembly2.Setup(a => a.FullName).Returns("Churchee.Module2");
            mockAssembly2.Setup(a => a.GetTypes()).Returns([typeof(TestModule2)]);

            var mockAssembly3 = new Mock<Assembly>();
            mockAssembly3.Setup(a => a.FullName).Returns("Other.Module3");
            mockAssembly3.Setup(a => a.GetTypes()).Returns([]);

            var assemblies = new List<Assembly> { mockAssembly1.Object, mockAssembly2.Object, mockAssembly3.Object };

            // Act
            var result = AssemblyResolution.GetModuleAssemblies(assemblies);

            // Assert
            result.Length.Should().Be(2);
        }

        private class TestModule1 : IModule
        {
            public string Name => "Module 1";
        }
        private class TestModule2 : IModule
        {
            public string Name => "Module 2";
        }
    }
}
