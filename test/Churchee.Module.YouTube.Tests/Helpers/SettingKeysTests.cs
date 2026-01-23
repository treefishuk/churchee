using System;
using Xunit;
using Churchee.Module.YouTube.Helpers;

namespace Churchee.Module.YouTube.Tests.Helpers
{
    public class SettingKeysTests
    {
        [Fact]
        public void Guid_Values_Are_Parsable()
        {
            Assert.NotEqual(Guid.Empty, SettingKeys.Handle);
            Assert.NotEqual(Guid.Empty, SettingKeys.ChannelId);
            Assert.NotEqual(Guid.Empty, SettingKeys.VideosPageName);
            Assert.Equal("b719d9ce-b561-4cbf-bb15-5933be775478", SettingKeys.ApiKeyToken);
        }
    }
}
