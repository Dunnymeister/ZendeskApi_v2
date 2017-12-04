using Xunit;
using ZendeskApi_v2;

namespace Tests
{
    public class AccountsAndActivityTests
    {
        private ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);

        [Fact]
        public void CanGetSettings()
        {
            var res = api.AccountsAndActivity.GetSettings();
            Assert.NotEmpty(res.Settings.Branding.HeaderColor);
        }

        [Fact]
        public void CanGetActivities()
        {
            //the api returns empty objects and I'm not sure how to get it to populate
            var res = api.AccountsAndActivity.GetActivities();

            //var res1 = api.AccountsAndActivity.GetActivityById()
            
        }
    }

    
}