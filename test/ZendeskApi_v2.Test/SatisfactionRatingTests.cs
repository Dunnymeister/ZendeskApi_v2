using Xunit;
using ZendeskApi_v2;

namespace Tests
{
    public class SatisfactionRatingTests
    {
        private ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);

        [Fact]
        public void CanGetSatisfactionRatings()
        {
            //there is no way to create satisfaction ratings through the api so they can't be tested
        }
    }
}