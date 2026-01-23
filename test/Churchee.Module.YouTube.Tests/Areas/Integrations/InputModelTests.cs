using Churchee.Module.YouTube.Areas.Integrations.Models;
using System.ComponentModel.DataAnnotations;

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
            object[] handleAttr = props.First(p => p.Name == "ChannelIdentifier").GetCustomAttributes(typeof(RequiredAttribute), false);
            object[] apiKeyAttr = props.First(p => p.Name == "ApiKey").GetCustomAttributes(typeof(RequiredAttribute), false);
            object[] nameAttr = props.First(p => p.Name == "NameForContent").GetCustomAttributes(typeof(RequiredAttribute), false);

            Assert.NotEmpty(handleAttr);
            Assert.NotEmpty(apiKeyAttr);
            Assert.NotEmpty(nameAttr);
        }
    }
}
