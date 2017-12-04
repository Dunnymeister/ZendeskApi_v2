using Xunit;
using ZendeskApi_v2;

namespace Tests
{
    public class TagTests
    {
        private ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);

        [Fact]
        public void CanGetTags()
        {
            var res = api.Tags.GetTags();

            Assert.True(res.Tags.Count > 0);
        }

        [Fact]
        public void CanAutocompleteTags()
        {
            var res = api.Tags.GetTags();
            var auto = api.Tags.AutocompleteTags(res.Tags[0].Name.Substring(0, 3));

            Assert.True(auto.Tags.Count > 0);
        }
    }
}