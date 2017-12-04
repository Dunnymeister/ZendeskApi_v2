using Xunit;
using ZendeskApi_v2;

namespace Tests
{
    public class SharingAgreementTests
    {
        private ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);

        [Fact]
        public void CanGetSharingAgreements()
        {
            var res = api.SharingAgreements.GetSharingAgreements();

            Assert.NotNull(res);
        }
    }
}