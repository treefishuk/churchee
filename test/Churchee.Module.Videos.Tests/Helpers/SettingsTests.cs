namespace Churchee.Module.Videos.Helpers
{
    public class SettingsTests
    {
        [Fact]
        public void VideosNameId_IsExpectedGuid()
        {
            Assert.Equal(Guid.Parse("4379e3d3-fa40-489b-b80d-01c30835fa9d"), typeof(Settings).GetField("VideosNameId")?.GetValue(null));
        }
    }
}
