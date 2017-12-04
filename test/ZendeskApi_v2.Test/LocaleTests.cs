
using Xunit;
using ZendeskApi_v2;

namespace Tests
{

    public class LocaleTests
    {
        private ZendeskApi api = new ZendeskApi(Settings.Site, Settings.AdminEmail, Settings.AdminPassword);

        [Fact]
        public void CanGetLocales()
        {
            var all = api.Locales.GetAllLocales();
            Assert.True(all.Count > 0);

            var agent = api.Locales.GetLocalesForAgents();
            Assert.True(agent.Count > 0);

            var specific = api.Locales.GetLocaleById(all.Locales[0].Id);
            Assert.Equal(specific.Locale.Id, all.Locales[0].Id);
            Assert.Null(specific.Locale.Translations);

            var specificWithTranslation = api.Locales.GetLocaleById(all.Locales[0].Id, true);
            Assert.Equal(specificWithTranslation.Locale.Id, all.Locales[0].Id);
            Assert.NotNull(specificWithTranslation.Locale.Translations);

            var current = api.Locales.GetCurrentLocale();
            Assert.True(current.Locale.Id > 0);
            Assert.Null(current.Locale.Translations);

            var currentWithTranslation = api.Locales.GetCurrentLocale(true);
            Assert.True(currentWithTranslation.Locale.Id > 0);
            Assert.NotNull(currentWithTranslation.Locale.Translations);
        }
    }
}