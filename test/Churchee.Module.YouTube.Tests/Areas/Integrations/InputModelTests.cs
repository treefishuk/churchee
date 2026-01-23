using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;
using Churchee.Module.YouTube.Areas.Integrations.Models;

namespace Churchee.Module.YouTube.Tests.Areas.Integrations
{
    public class InputModelTests
    {
        [Fact]
        public void DefaultConstructor_Initializes_Strings()
        {
            var model = new InputModel();

            Assert.NotNull(model.ChannelIdentifier);
            Assert.NotNull(model.ApiKey);
            Assert.NotNull(model.NameForContent);
            Assert.Equal(string.Empty, model.ChannelIdentifier);
            Assert.Equal(string.Empty, model.ApiKey);
            Assert.Equal(string.Empty, model.NameForContent);
        }

        [Fact]
        public void Properties_Have_Required_Attributes()
        {
            var props = typeof(InputModel).GetProperties();
            var handleAttr = props.First(p => p.Name == "Handle").GetCustomAttributes(typeof(RequiredAttribute), false);
            var apiKeyAttr = props.First(p => p.Name == "ApiKey").GetCustomAttributes(typeof(RequiredAttribute), false);
            var nameAttr = props.First(p => p.Name == "NameForContent").GetCustomAttributes(typeof(RequiredAttribute), false);

            Assert.NotEmpty(handleAttr);
            Assert.NotEmpty(apiKeyAttr);
            Assert.NotEmpty(nameAttr);
        }
    }
}
